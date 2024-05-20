using BusinessLogic.Services.Core.Base;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.UserAuths.Incomings;
using WebApi.DTOs.Implementation.UserAuths.Outgoings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Controllers.UserAuths
{
    /// <summary>
    ///     Controller that handles some operations related to
    ///     user login problems.
    /// </summary>
    [Route("api/auth/user")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserAuthService _userAuthService;
        private readonly IUserTokenService _userTokenService;
        private readonly SignInManager<UserEntity> _signInManager;

        public UserAuthController(
            IUserService userService,
            IUserAuthService userAuthService,
            IUserTokenService userTokenService,
            SignInManager<UserEntity> signInManager)
        {
            _userService = userService;
            _userAuthService = userAuthService;
            _userTokenService = userTokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            UserLoginDto loginDto,
            CancellationToken cancellationToken)
        {
            loginDto.NormalizeAllProperties();

            var hasRegisterd = await _userAuthService.IsEmailRegisteredAsync(
                email: loginDto.Email,
                cancellationToken: cancellationToken);

            if (!hasRegisterd)
            {
                return NotFound(ApiResponse.Failed($"No account with email [{loginDto.Email}] has registed."));
            }

            var isUserHasConfirmed = await _userService.CheckRegistrationConfirmationByEmailAsync(
                email: loginDto.Email,
                cancellationToken: cancellationToken);

            if (!isUserHasConfirmed)
            {
                return BadRequest(ApiResponse.Failed($"User [{loginDto.Email}] has not been confirmed the registration."));
            }

            var isUserBanned = await _userService.CheckUserBannedStatusByEmailAsync(
                email: loginDto.Email,
            cancellationToken: cancellationToken);

            if (isUserBanned)
            {
                return BadRequest(ApiResponse.Failed("User has been banned by the admin."));
            }

            var foundUser = await _userService.FindUserByEmailAsync(email: loginDto.Email);
            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user: foundUser,
                password: loginDto.Password,
                lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return BadRequest(ApiResponse.Failed("You has been locked out due to attempts is exceed 3 times."));
                }

                return BadRequest(ApiResponse.Failed("Invalid login credentials."));
            }

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
            var accessToken = _userTokenService.GenerateAccessToken(
                user: foundUser,
                tokenId: refreshTokenId);

            var refreshToken = _userTokenService.GenerateRefreshToken(
                user: foundUser,
                tokenId: refreshTokenId);

            var loginResponseDto = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            // Set the guestId cookie and guestId from shopping cart
            // to this signed-in userId.
            HttpContextHelper.SetGuestId(HttpContext, foundUser.Id);
            ShoppingCartHelper.SetGuestIdToShoppingCart(HttpContext, foundUser.Id);

            return Ok(ApiResponse.Success(loginResponseDto));
        }

        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(
            [FromBody]
            UserLogoutDto logoutDto,
            CancellationToken cancellationToken)
        {
            // Validate the refresh token.
            var refreshToken = logoutDto.RefreshToken;

            var refreshTokenValidationResult = await _userTokenService.ValidateRefreshTokenAsync(
                refreshToken: refreshToken,
                cancellationToken: cancellationToken);

            if (!refreshTokenValidationResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(refreshTokenValidationResult.ErrorMessages));
            }

            // Get credentials that encapsulated from the refresh token to use later for the remove.
            var refreshTokenCredentials = refreshTokenValidationResult.Value;

            var removeRefreshTokenResult = await _userTokenService.RemoveUserTokenByIdAsync(
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
