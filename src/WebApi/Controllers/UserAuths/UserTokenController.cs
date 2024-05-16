using BusinessLogic.Services.Core.Base;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.UserAuths.Incomings;
using WebApi.Models;

namespace WebApi.Controllers.UserAuths
{
    /// <summary>
    ///     Controller that handles some operations related to
    ///     userToken problems.
    /// </summary>
    [Route("api/auth/user/token")]
    [ApiController]
    public class UserTokenController : ControllerBase
    {
        private readonly IUserTokenService _userTokenService;
        private readonly IUserService _userService;

        public UserTokenController(
            IUserTokenService userTokenService,
            IUserService userService)
        {
            _userTokenService = userTokenService;
            _userService = userService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAccessTokenAsync(
            [FromBody]
            RefreshAccessTokenDto refreshAccessTokenDto,
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
                && refreshTokenCredentials.UserId.Equals(accessTokenCredentials.UserId);

            if (!isValidToDoRefresh)
            {
                return BadRequest(ApiResponse.Failed("The refresh-token and access-token credentials are not matched."));
            }

            var removeRefreshTokenResult = await _userTokenService.RemoveUserTokenByIdAsync(
                tokenId: refreshTokenCredentials.Id,
                cancellationToken: cancellationToken);

            if (!removeRefreshTokenResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            var foundUser = await _userService.FindUserByIdForProfileDisplayAsync(
                userId: refreshTokenCredentials.UserId,
                cancellationToken: cancellationToken);

            // Create the refresh token for the login user and save to database.
            var refreshTokenId = Guid.NewGuid();

            var userRefreshToken = _userTokenService.CreateUserTokenByPurpose(
                tokenId: refreshTokenId,
                user: foundUser,
                tokenPurpose: BusinessLogic.Enums.TokenPurpose.RefreshToken);

            var saveResult = await _userTokenService.SaveUserTokenAsync(
                userToken: userRefreshToken,
                cancellationToken: cancellationToken);

            if (!saveResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.DatabaseError));
            }

            // Generate the access token and refresh token for user.
            accessToken = _userTokenService.GenerateAccessToken(
                user: foundUser,
                tokenId: refreshTokenId);

            refreshToken = _userTokenService.GenerateRefreshToken(
                user: foundUser,
                tokenId: refreshTokenId);

            var responseDto = new RefreshAccessTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return Ok(ApiResponse.Success(responseDto));
        }
    }
}
