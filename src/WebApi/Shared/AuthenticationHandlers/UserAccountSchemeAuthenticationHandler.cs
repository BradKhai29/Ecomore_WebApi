using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Options.Models.Jwts.UserAccount;
using System.Security.Claims;
using System.Text.Encodings.Web;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Shared.AuthenticationHandlers
{
    /// <summary>
    ///     The authentication handler for <see cref="CustomAuthenticationSchemes.UserAccountScheme"/>.
    /// </summary>
    public sealed class UserAccountSchemeAuthenticationHandler :
        AuthenticationHandler<AuthenticationSchemeOptions>
    {
        // Backing fields.
        private readonly AccessTokenOptions _accessTokenOptions;
        private readonly SecurityTokenHandler _securityTokenHandler;

        public UserAccountSchemeAuthenticationHandler(
            AccessTokenOptions accessTokenOptions,
            SecurityTokenHandler securityTokenHandler,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _accessTokenOptions = accessTokenOptions;
            _securityTokenHandler = securityTokenHandler;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers.Authorization;

            var isValidHeader = VerifyAuthorizationHeader(authorizationHeader);

            if (!isValidHeader)
            {
                await WriteUnauthorizedResponseAsync(Context);

                return AuthenticateResult.Fail(failureMessage: $"Missing header: {JwtBearerDefaults.AuthenticationScheme}");
            }

            var accessToken = authorizationHeader
                .ToString()
                .Split(separator: JwtBearerDefaults.AuthenticationScheme, StringSplitOptions.TrimEntries)
                .Last();

            var tokenValidationResult = await ValidateTokenAsync(accessToken);

            if (!tokenValidationResult.IsValid)
            {
                await WriteUnauthorizedResponseAsync(Context);

                return AuthenticateResult.Fail(failureMessage: $"Invalid token.");
            }

            var authenticationTicket = CreateTicketFromValidationResult(tokenValidationResult);

            return AuthenticateResult.Success(ticket: authenticationTicket);
        }

        #region Private Methods
        /// <summary>
        ///  Reference: https://github.com/dotnet/aspnetcore/issues/44100
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task WriteUnauthorizedResponseAsync(HttpContext httpContext)
        {
            var unauthorizedResponse = ApiResponse.Failed(
                errorMessages: ApiResponse.DefaultMessage.InvalidAccessToken);

            httpContext.Response.Clear();
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(unauthorizedResponse);
            await httpContext.Response.CompleteAsync();
        }

        private bool VerifyAuthorizationHeader(string authorizationHeader)
        {
            var isNotEmpty = !string.IsNullOrEmpty(authorizationHeader);

            if (!isNotEmpty)
            {
                return false;
            }

            var hasBearer = authorizationHeader.Contains(
                value: JwtBearerDefaults.AuthenticationScheme);

            if (!hasBearer)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Validate the parameters of the input token is valid 
        ///     to this application's requirements or not.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<TokenValidationResult> ValidateTokenAsync(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _accessTokenOptions.Issuer,
                ValidAudience = _accessTokenOptions.Audience,
                IssuerSigningKey = _accessTokenOptions.GetSecurityKey(),
            };

            // Validate the token credentials.
            var validationResult = await _securityTokenHandler.ValidateTokenAsync(
                token: token,
                validationParameters: tokenValidationParameters);

            return validationResult;
        }

        private AuthenticationTicket CreateTicketFromValidationResult(TokenValidationResult validationResult)
        {
            var claimsPrincipal = new ClaimsPrincipal(identity: validationResult.ClaimsIdentity);
            var authenticationScheme = CustomAuthenticationSchemes.UserAccountScheme;

            var authenticationTicket = new AuthenticationTicket(
                principal: claimsPrincipal,
                authenticationScheme: authenticationScheme);

            return authenticationTicket;
        }
        #endregion
    }
}
