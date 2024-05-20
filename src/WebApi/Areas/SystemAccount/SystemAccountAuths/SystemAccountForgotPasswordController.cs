using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Implementation.SystemAccountAuths.Incomings;
using WebApi.DTOs.Implementation.SystemAccountAuths.Outgoings;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Areas.SystemAccount.SystemAccountAuths
{
    [Route("api/auth/system/forgot-password")]
    [ApiController]
    public class SystemAccountForgotPasswordController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountService;
        private readonly ISystemAccountTokenService _systemAccountTokenService;
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemAccountForgotPasswordController(
            ISystemAccountService systemAccountService,
            ISystemAccountTokenService systemAccountTokenService,
            IMailService mailService,
            IWebHostEnvironment webHostEnvironment)
        {
            _systemAccountService = systemAccountService;
            _systemAccountTokenService = systemAccountTokenService;
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
            var foundAccount = await _systemAccountService.FindAccountByEmailAsync(
                email: email,
                cancellationToken: cancellationToken);

            if (Equals(foundAccount, null))
            {
                return BadRequest(ApiResponse.Failed($"No user with email [{email}] has been registered."));
            }

            var resetPasswordTokenId = Guid.NewGuid();

            var resetPasswordSystemAccountToken = _systemAccountTokenService.CreateSystemAccountTokenByPurpose(
                tokenId: resetPasswordTokenId,
                systemAccount: foundAccount,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.ResetPasswordToken);

            var saveTokenResult = await _systemAccountTokenService.SaveSystemAccountTokenAsync(
                resetPasswordSystemAccountToken,
                cancellationToken: cancellationToken);

            if (!saveTokenResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            var resetPasswordToken = _systemAccountTokenService.GenerateResetPasswordToken(
                systemAccount: foundAccount,
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
            var validationResult = await _systemAccountTokenService.ValidateResetPasswordTokenAsync(
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
            ResetPasswordDto resetPasswordDto,
            CancellationToken cancellationToken)
        {
            var resetPasswordToken = resetPasswordDto.ResetPasswordToken;

            var validationResult = await _systemAccountTokenService.ValidateResetPasswordTokenAsync(
                resetPasswordToken: resetPasswordToken,
                cancellationToken: cancellationToken);

            if (!validationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(validationResult.ErrorMessages));
            }

            var tokenCredentials = validationResult.Value;

            var updateResult = await _systemAccountService.UpdatePasswordByAccountIdAsync(
                systemAccountId: tokenCredentials.SystemAccountId,
                newPassword: resetPasswordDto.Password,
                cancellationToken: cancellationToken);

            if (!updateResult)
            {
                return BadRequest(ApiResponse.Failed("Failed to update user password."));
            }

            await _systemAccountTokenService.RemoveSystemAccountTokenByIdAsync(
                tokenId: tokenCredentials.Id,
                cancellationToken: cancellationToken);

            return Ok(ApiResponse.Success(default));
        }
    }
}
