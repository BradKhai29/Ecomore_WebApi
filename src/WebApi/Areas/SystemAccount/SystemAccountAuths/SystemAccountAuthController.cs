using BusinessLogic.Services.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.SystemAccountAuths.Incomings;
using WebApi.DTOs.Implementation.SystemAccountAuths.Outgoings;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Areas.SystemAccount.SystemAccountAuths
{
    [Route("api/auth/system")]
    [ApiController]
    public class SystemAccountAuthController : ControllerBase
    {
        private readonly ISystemAccountAuthService _systemAccountAuthService;
        private readonly ISystemAccountService _systemAccountService;
        private readonly ISystemAccountTokenService _systemAccountTokenService;

        public SystemAccountAuthController(
            ISystemAccountAuthService systemAccountAuthService,
            ISystemAccountService systemAccountService,
            ISystemAccountTokenService systemAccountTokenService)
        {
            _systemAccountAuthService = systemAccountAuthService;
            _systemAccountService = systemAccountService;
            _systemAccountTokenService = systemAccountTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            SystemAccountLoginDto loginDto,
            CancellationToken cancellationToken)
        {
            loginDto.NormalizeAllProperties();

            var isEmailRegistered = await _systemAccountAuthService.IsEmailRegisteredAsync(
                email: loginDto.Email,
                cancellationToken: cancellationToken);

            if (!isEmailRegistered)
            {
                return NotFound(ApiResponse.Failed($"No account with email [{loginDto.Email}] has registed."));
            }

            var hasRegisteredConfirmed = await _systemAccountService.CheckRegistrationConfirmationByEmailAsync(
                email: loginDto.Email,
                cancellationToken: cancellationToken);

            if (!hasRegisteredConfirmed)
            {
                return BadRequest(ApiResponse.Failed($"Account with Email [{loginDto.Email}] has not been confirmed the registration."));
            }

            var loginResult = await _systemAccountAuthService.LoginAsync(
                email: loginDto.Email,
                password: loginDto.Password,
                cancellationToken: cancellationToken);

            if (!loginResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(loginResult.ErrorMessages));
            }

            var foundSystemAccount = loginResult.Value;

            // Create the refresh token for the login user and save to database.
            var refreshTokenId = Guid.NewGuid();

            var userRefreshToken = _systemAccountTokenService.CreateSystemAccountTokenByPurpose(
                tokenId: refreshTokenId,
                systemAccount: foundSystemAccount,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.RefreshToken);

            var saveResult = await _systemAccountTokenService.SaveSystemAccountTokenAsync(
                systemAccountToken: userRefreshToken,
                cancellationToken: cancellationToken);

            if (!saveResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            // Generate the access token and refresh token for user.
            var accessToken = _systemAccountTokenService.GenerateAccessToken(
                systemAccount: foundSystemAccount,
                tokenId: refreshTokenId);

            var refreshToken = _systemAccountTokenService.GenerateRefreshToken(
                systemAccount: foundSystemAccount,
                tokenId: refreshTokenId);

            var loginResponseDto = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return Ok(ApiResponse.Success(loginResponseDto));
        }

        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.SystemAccountScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(
            [FromBody]
            SystemAccountLogoutDto logoutDto,
            CancellationToken cancellationToken)
        {
            // Validate the refresh token.
            var refreshToken = logoutDto.RefreshToken;

            var refreshTokenValidationResult = await _systemAccountTokenService.ValidateRefreshTokenAsync(
                refreshToken: refreshToken,
                cancellationToken: cancellationToken);

            if (!refreshTokenValidationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(refreshTokenValidationResult.ErrorMessages));
            }

            // Get credentials that encapsulated from the refresh token to use later for the remove.
            var refreshTokenCredentials = refreshTokenValidationResult.Value;

            var removeRefreshTokenResult = await _systemAccountTokenService.RemoveSystemAccountTokenByIdAsync(
                tokenId: refreshTokenCredentials.Id,
                cancellationToken: cancellationToken);

            if (!removeRefreshTokenResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            return Ok(ApiResponse.Success(default));
        }
    }
}
