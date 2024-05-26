using BusinessLogic.Services.Core.Base;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.SystemAccountAuths.Incomings;
using WebApi.Models;

namespace WebApi.Areas.SystemAccount.SystemAccountAuths
{
    [Route("api/auth/system/token")]
    [ApiController]
    public class SystemAccountTokenController : ControllerBase
    {
        private readonly ISystemAccountTokenService _userTokenService;

        public SystemAccountTokenController(ISystemAccountTokenService userTokenService)
        {
            _userTokenService = userTokenService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAccessTokenAsync(
            [FromBody]
            SystemAccountRefreshAccessTokenDto refreshAccessTokenDto,
            CancellationToken cancellationToken)
        {
            // Validate the access token.
            var accessToken = refreshAccessTokenDto.AccessToken;

            var accessTokenValidationResult = await _userTokenService.ValidateAccessTokenAsync(
                accessToken: accessToken,
                cancellationToken: cancellationToken);

            if (!accessTokenValidationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(accessTokenValidationResult.ErrorMessages));
            }

            var accessTokenCredentials = accessTokenValidationResult.Value;

            var isAccessTokenExpired = accessTokenCredentials.ExpiredAt < DateTime.Now;

            if (!isAccessTokenExpired)
            {
                return BadRequest(ApiResponse.Failed("The access token is not expired yet."));
            }

            // Validate the refresh token.
            var refreshToken = refreshAccessTokenDto.RefreshToken;

            var refreshTokenValidationResult = await _userTokenService.ValidateRefreshTokenAsync(
                refreshToken: refreshToken,
                cancellationToken: cancellationToken);

            if (!refreshTokenValidationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(refreshTokenValidationResult.ErrorMessages));
            }

            var refreshTokenCredentials = refreshTokenValidationResult.Value;

            var isValidToDoRefresh = refreshTokenCredentials.Id.Equals(accessTokenCredentials.Id)
                && refreshTokenCredentials.SystemAccountId.Equals(accessTokenCredentials.SystemAccountId);

            if (!isValidToDoRefresh)
            {
                return BadRequest(ApiResponse.Failed("The refresh-token and access-token credentials are not matched."));
            }

            var removeRefreshTokenResult = await _userTokenService.RemoveSystemAccountTokenByIdAsync(
                tokenId: refreshTokenCredentials.Id,
                cancellationToken: cancellationToken);

            if (!removeRefreshTokenResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            var foundAccount = new SystemAccountEntity
            {
                Id = refreshTokenCredentials.SystemAccountId,
            };

            // Create the refresh token for the login user and save to database.
            var refreshTokenId = Guid.NewGuid();

            var userRefreshToken = _userTokenService.CreateSystemAccountTokenByPurpose(
                tokenId: refreshTokenId,
                systemAccount: foundAccount,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.RefreshToken);

            var saveResult = await _userTokenService.SaveSystemAccountTokenAsync(
                systemAccountToken: userRefreshToken,
                cancellationToken: cancellationToken);

            if (!saveResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            // Generate the access token and refresh token for user.
            accessToken = _userTokenService.GenerateAccessToken(
                systemAccount: foundAccount,
                tokenId: refreshTokenId);

            refreshToken = _userTokenService.GenerateRefreshToken(
                systemAccount: foundAccount,
                tokenId: refreshTokenId);

            var responseDto = new SystemAccountRefreshAccessTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return Ok(ApiResponse.Success(responseDto));
        }
    }
}
