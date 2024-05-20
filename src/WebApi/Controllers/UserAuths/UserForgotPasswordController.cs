using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Implementation.SystemAccountAuths.Incomings;
using WebApi.DTOs.Implementation.UserAuths.Incomings;
using WebApi.DTOs.Implementation.UserAuths.Outgoings;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Controllers.UserAuths
{
    [Route("api/auth/user/forgot-password")]
    [ApiController]
    public class UserForgotPasswordController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserTokenService _userTokenService;
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserForgotPasswordController(
            IUserService userService,
            IUserTokenService userTokenService,
            IMailService mailService,
            IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _userTokenService = userTokenService;
            _mailService = mailService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("{email}")]
        public async Task<IActionResult> SendResetPasswordLinkAsync(
            [FromRoute]
            [EmailAddress]
            [Required] string email,
            CancellationToken cancellationToken)
        {
            var isEmailConfirmed = await _userService.CheckRegistrationConfirmationByEmailAsync(
                email: email,
                cancellationToken: cancellationToken);

            if (!isEmailConfirmed)
            {
                return BadRequest(ApiResponse.Failed($"Account with email [{email}] has not confirmed."));
            }

            var foundUser = await _userService.FindUserByEmailAsync(email: email);

            if (Equals(foundUser, null))
            {
                return BadRequest(ApiResponse.Failed($"No user with email [{email}] has been registered."));
            }

            var resetPasswordTokenId = Guid.NewGuid();

            var resetPasswordUserToken = _userTokenService.CreateUserTokenByPurpose(
                tokenId: resetPasswordTokenId,
                user: foundUser,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.ResetPasswordToken);

            var saveTokenResult = await _userTokenService.SaveUserTokenAsync(
                userToken: resetPasswordUserToken,
                cancellationToken: cancellationToken);

            if (!saveTokenResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            var resetPasswordToken = _userTokenService.GenerateResetPasswordToken(
                user: foundUser,
                tokenId: resetPasswordTokenId);

            var mailTemplatePath = Path.Combine(
                path1: _webHostEnvironment.ContentRootPath,
                path2: AppFolders.Others,
                path3: AppFolders.MailTemplate,
                path4: AppFiles.ResetPasswordMailTemplateFile);

            var mailContent = await _mailService.BuildRegisterConfirmMailContentAsync(
                templatePath: mailTemplatePath,
                to: email,
                subject: $"Dat lai mat khau cho tai khoan Ecomore.",
                confirmationLink: $"https://localhost:7106/api/auth/user/forgot-password?token={resetPasswordToken}");

            var sendResult = await _mailService.SendMailAsync(mailContent: mailContent);

            if (!sendResult)
            {
                return BadRequest(ApiResponse.Failed(ApiResponse.DefaultMessage.FailedToSendEmail));
            }

            return Ok(ApiResponse.Success(default));
        }

        [HttpGet]
        public async Task<IActionResult> VerifyResetPasswordTokenAsync(
            [FromQuery(Name = "token")]
            [Required] string resetPasswordToken,
            CancellationToken cancellationToken)
        {
            var validationResult = await _userTokenService.ValidateResetPasswordTokenAsync(
                resetPasswordToken: resetPasswordToken,
                cancellationToken: cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(validationResult.ErrorMessages));
            }

            return Ok(ApiResponse.Success(new ResetPasswordResponseDto
            {
                ResetPasswordToken = resetPasswordToken
            }));
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(
            UserResetPasswordDto resetPasswordDto,
            CancellationToken cancellationToken)
        {
            var resetPasswordToken = resetPasswordDto.ResetPasswordToken;

            var validationResult = await _userTokenService.ValidateResetPasswordTokenAsync(
                resetPasswordToken: resetPasswordToken,
                cancellationToken: cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(validationResult.ErrorMessages));
            }

            var tokenCredentials = validationResult.Value;

            var updateResult = await _userService.UpdateUserPasswordAsyncByUserId(
                userId: tokenCredentials.UserId,
                newPassword: resetPasswordDto.Password,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                return BadRequest(ApiResponse.Failed("Failed to update user password."));
            }

            await _userTokenService.RemoveUserTokenByIdAsync(
                tokenId: tokenCredentials.Id,
                cancellationToken: cancellationToken);

            return Ok(ApiResponse.Success(default));
        }
    }
}
