using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.Commons.SystemConstants;
using DataAccess.DataSeedings;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Implementation.UserAuths.Incomings;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Controllers.UserAuths
{
    /// <summary>
    ///     Controller that handles some operations related to
    ///     user registration problems.
    /// </summary>
    [Route("api/auth/user/register")]
    [ApiController]
    public class UserRegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserAuthService _userAuthService;
        private readonly IMailService _mailService;
        private readonly IUserTokenService _userTokenService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserRegisterController(
            IUserService userService,
            IUserAuthService userAuthService,
            IMailService mailService,
            IUserTokenService userTokenService,
            IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _userAuthService = userAuthService;
            _mailService = mailService;
            _userTokenService = userTokenService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync(
            UserRegisterDto registerDto,
            CancellationToken cancellationToken)
        {
            registerDto.NormalizeAllProperties();

            // Check if the registered email is existed or not.
            var isEmailExisted = await _userAuthService.IsEmailRegisteredAsync(
                email: registerDto.Email,
                cancellationToken: cancellationToken);

            if (isEmailExisted)
            {
                return BadRequest(
                    error: ApiResponse.Failed($"Email [{registerDto.Email}] is already existed."));
            }

            // Register the new user account.
            var newUser = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = UserEntity.GetFullName(registerDto.FirstName, registerDto.LastName),
                // Set the status for the new user account.
                AccountStatusId = AccountStatuses.PendingConfirmed.Id,
                AvatarUrl = DefaultValues.UserAvatarUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Validate if the password is conform the format or not.
            var passwordValidationResult = await _userAuthService.ValidatePasswordAsync(
                user: newUser,
                password: registerDto.Password);

            if (!passwordValidationResult.Succeeded)
            {
                var messages = passwordValidationResult.Errors.Select(error => error.Description);

                return BadRequest(ApiResponse.Failed(messages));
            }

            var registerResult = await _userAuthService.RegisterUserAsync(
                newUser: newUser,
                password: registerDto.Password,
                cancellationToken: cancellationToken);

            if (!registerResult)
            {
                return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            // Generate the register confirmation token.
            var tokenId = Guid.NewGuid();

            var registerConfirmationToken = _userTokenService.GenerateRegisterConfirmationToken(
                user: newUser,
                tokenId: tokenId);

            var userToken = _userTokenService.CreateUserTokenByPurpose(
                tokenId: tokenId,
                user: newUser,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.RegisterConfirmationToken);

            var saveResult = await _userTokenService.SaveUserTokenAsync(userToken: userToken,
                cancellationToken: cancellationToken);

            if (!saveResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            var mailTemplatePath = Path.Combine(
                path1: _webHostEnvironment.ContentRootPath,
                path2: AppFolders.Others,
                path3: AppFolders.MailTemplate,
                path4: AppFiles.RegisterConfirmationMailTemplateFile);

            var mailContent = await _mailService.BuildRegisterConfirmMailContentAsync(
                templatePath: mailTemplatePath,
                to: registerDto.Email,
                subject: $"Xac nhan hoan tat dang ky tai khoan Ecomore.",
                confirmationLink: $"https://localhost:7106/api/auth/user/register/confirm?token={registerConfirmationToken}");

            var sendResult = await _mailService.SendMailAsync(mailContent: mailContent);

            if (!sendResult)
            {
                return BadRequest(ApiResponse.Failed(ApiResponse.DefaultMessage.FailedToSendEmail));
            }

            return Ok(ApiResponse.Success(default));
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> RegisterConfirmAsync(
            [FromQuery(Name = "token")]
            [Required] string registerConfirmationToken,
            CancellationToken cancellationToken)
        {
            var validationResult = await _userTokenService.ValidateRegisterConfirmationTokenAsync(
                confirmationToken: registerConfirmationToken,
                cancellationToken: cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(validationResult.ErrorMessages));
            }

            var userToken = validationResult.Value;

            var confirmationResult = await _userAuthService.ConfirmUserRegistrationAsync(
                user: userToken.User,
                cancellationToken: cancellationToken);

            if (!confirmationResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            await _userTokenService.RemoveUserTokenByIdAsync(
                tokenId: userToken.Id,
                cancellationToken: cancellationToken);

            return Ok(ApiResponse.Success(default));
        }

        [HttpPost("resend-confirm")]
        public async Task<IActionResult> ResendRegisterConfirmationEmailAsync(
            [Required]
            string email,
            CancellationToken cancellationToken)
        {
            var hasRegistered = await _userAuthService.IsEmailRegisteredAsync(
            email: email,
                cancellationToken: cancellationToken);

            if (!hasRegistered)
            {
                return NotFound(ApiResponse.Failed($"No account with email [{email}] has registed."));
            }

            var isUserHasConfirmed = await _userService.CheckRegistrationConfirmationByEmailAsync(
            email: email,
                cancellationToken: cancellationToken);

            if (isUserHasConfirmed)
            {
                return Conflict(ApiResponse.Failed($"User [{email}] has confirmed the registration."));
            }

            var foundUser = await _userService.FindUserByEmailAsync(email: email);

            // Remove all the register confirmation token from the database to create new token for next confirmation.
            var removeResult = await _userTokenService.RemoveAllTokensByTokenPurposeAsync(
                userId: foundUser.Id,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.RegisterConfirmationToken,
                cancellationToken: cancellationToken);

            if (!removeResult)
            {
                return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            // Generate the register confirmation token.
            var tokenId = Guid.NewGuid();

            var registerConfirmationToken = _userTokenService.GenerateRegisterConfirmationToken(
                user: foundUser,
                tokenId: tokenId);

            var userToken = _userTokenService.CreateUserTokenByPurpose(
                tokenId: tokenId,
                user: foundUser,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.RegisterConfirmationToken);

            var saveResult = await _userTokenService.SaveUserTokenAsync(userToken: userToken,
                cancellationToken: cancellationToken);

            if (!saveResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            var mailTemplatePath = Path.Combine(
                path1: _webHostEnvironment.ContentRootPath,
                path2: AppFolders.Others,
                path3: AppFolders.MailTemplate,
                path4: AppFiles.RegisterConfirmationMailTemplateFile);

            var mailContent = await _mailService.BuildRegisterConfirmMailContentAsync(
                templatePath: mailTemplatePath,
                to: foundUser.Email,
                subject: $"Xac nhan hoan tat dang ky tai khoan Ecomore.",
                confirmationLink: $"https://localhost:7106/api/auth/user/register/confirm?token={registerConfirmationToken}");

            var sendResult = await _mailService.SendMailAsync(mailContent: mailContent);

            if (!sendResult)
            {
                return BadRequest(ApiResponse.Failed(ApiResponse.DefaultMessage.FailedToSendEmail));
            }

            return Ok(ApiResponse.Success(default));
        }
    }
}
