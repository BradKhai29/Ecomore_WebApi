using BusinessLogic.Commons;
using BusinessLogic.Enums;
using BusinessLogic.Models;
using BusinessLogic.Services.Core.Base;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Options.Models.Jwts.UserAccount;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class UserTokenService : IUserTokenService
    {
        // Backing fields.
        private readonly SecurityTokenHandler _securityTokenHandler;
        private readonly AccessTokenOptions _accessTokenOptions;
        private readonly RefreshTokenOptions _refreshTokenOptions;
        private readonly RegisterConfirmationTokenOptions _registerConfirmationTokenOptions;
        private readonly ResetPasswordTokenOptions _resetPasswordTokenOptions;
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public UserTokenService(
            AccessTokenOptions jwtOptions,
            SecurityTokenHandler securityTokenHandler,
            IUnitOfWork<AppDbContext> unitOfWork,
            ResetPasswordTokenOptions resetPasswordOptions,
            RefreshTokenOptions refreshTokenOptions,
            RegisterConfirmationTokenOptions registerConfirmationTokenOptions)
        {
            _accessTokenOptions = jwtOptions;
            _securityTokenHandler = securityTokenHandler;
            _unitOfWork = unitOfWork;
            _resetPasswordTokenOptions = resetPasswordOptions;
            _refreshTokenOptions = refreshTokenOptions;
            _registerConfirmationTokenOptions = registerConfirmationTokenOptions;
        }

        public string GenerateAccessToken(
            UserEntity user,
            Guid tokenId)
        {
            var claims = new List<Claim>
            {
                new(type: JwtRegisteredClaimNames.Iat, value: DateTime.Now.ToString()),
                new(type: JwtRegisteredClaimNames.Jti, tokenId.ToString()),
                new(type: UserCustomClaimTypes.UserId, user.Id.ToString()),
                new(type: UserCustomClaimTypes.FullName, user.GetFullNameWithoutSeparator()),
                new(type: UserCustomClaimTypes.AvatarUrl, user.AvatarUrl)
            };

            var tokenDescriptor = InitTokenDescriptorByTokenPurpose(
                claims: claims,
                lifeSpan: TimeSpan.FromMinutes(_accessTokenOptions.LiveMinutes),
                tokenPurpose: TokenPurpose.AccessToken);

            // Generate the token.
            var token = _securityTokenHandler.CreateToken(tokenDescriptor: tokenDescriptor);

            return _securityTokenHandler.WriteToken(token);
        }


        public string GenerateRefreshToken(
            UserEntity user,
            Guid tokenId)
        {
            var claims = new List<Claim>
            {
                new(type: JwtRegisteredClaimNames.Iat, value: DateTime.Now.ToString()),
                new(type: JwtRegisteredClaimNames.Jti, tokenId.ToString()),
                new(type: UserCustomClaimTypes.UserId, user.Id.ToString()),
            };

            var tokenDescriptor = InitTokenDescriptorByTokenPurpose(
                claims: claims,
                lifeSpan: TimeSpan.FromDays(_refreshTokenOptions.LiveDays),
                tokenPurpose: TokenPurpose.RefreshToken);

            // Generate the token.
            var token = _securityTokenHandler.CreateToken(tokenDescriptor: tokenDescriptor);

            return _securityTokenHandler.WriteToken(token);
        }

        public string GenerateRegisterConfirmationToken(
            UserEntity user,
            Guid tokenId)
        {
            var claims = new List<Claim>()
            {
                new(type: JwtRegisteredClaimNames.Jti, tokenId.ToString()),
                new(type: UserCustomClaimTypes.UserId, user.Id.ToString())
            };

            var tokenDescriptor = InitTokenDescriptorByTokenPurpose(
                claims: claims,
                lifeSpan: TimeSpan.FromMinutes(_registerConfirmationTokenOptions.LiveMinutes),
                tokenPurpose: TokenPurpose.RegisterConfirmationToken);

            // Generate the token.
            var token = _securityTokenHandler.CreateToken(tokenDescriptor: tokenDescriptor);

            return _securityTokenHandler.WriteToken(token);
        }

        public string GenerateResetPasswordToken(UserEntity user, Guid tokenId)
        {
            var claims = new List<Claim>()
            {
                new(type: JwtRegisteredClaimNames.Jti, tokenId.ToString()),
                new(type: UserCustomClaimTypes.UserId, user.Id.ToString())
            };

            var tokenDescriptor = InitTokenDescriptorByTokenPurpose(
                claims: claims,
                lifeSpan: TimeSpan.FromMinutes(_resetPasswordTokenOptions.LiveMinutes),
                tokenPurpose: TokenPurpose.ResetPasswordToken);

            // Generate the token.
            var token = _securityTokenHandler.CreateToken(tokenDescriptor: tokenDescriptor);

            return _securityTokenHandler.WriteToken(token);
        }

        public async Task<bool> SaveUserTokenAsync(
            UserTokenEntity userToken,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(operation: async () =>
            {
                await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                try
                {
                    await _unitOfWork.UserTokenRepository.AddAsync(
                        newEntity: userToken,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.SaveChangesToDatabaseAsync(
                        cancellationToken: cancellationToken
                    );

                    await _unitOfWork.CommitTransactionAsync(
                        cancellationToken: cancellationToken);

                    result = true;
                }
                catch
                {
                    await _unitOfWork.RollBackTransactionAsync(
                        cancellationToken: cancellationToken
                    );
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(
                        cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public async Task<AppResult<UserTokenEntity>> ValidateAccessTokenAsync(
            string accessToken,
            CancellationToken cancellationToken)
        {
            var validationResult = await InternalValidateTokenAsync(
                token: accessToken,
                tokenPurpose: TokenPurpose.AccessToken,
                validateLifeTime: false);

            if (!validationResult.IsValid)
            {
                return AppResult<UserTokenEntity>.Failed(AppResult.DefaultMessage.InvalidAccessToken);
            }

            var userIdClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(UserCustomClaimTypes.UserId));

            var jtiClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(JwtRegisteredClaimNames.Jti));

            var expClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(JwtRegisteredClaimNames.Exp));

            if (Equals(userIdClaim, null) || Equals(jtiClaim, null) || Equals(expClaim, null))
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId or Exp claim has invalid format.");
            }

            var isValidUserId = Guid.TryParse(userIdClaim.Value, out Guid userId);
            var isValidTokenId = Guid.TryParse(jtiClaim.Value, out Guid tokenId);
            var isValidExpClaim = long.TryParse(expClaim.Value, out long expInSeconds);

            if (!isValidUserId || !isValidTokenId || !isValidExpClaim)
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId or Exp claim has invalid format.");
            }

            // Check if the userId that encapsulated in this accessToken is existed or not.
            var isUserFound = await _unitOfWork.UserRepository.IsFoundByExpressionAsync(
                expression: user => user.Id == userId,
                cancellationToken: cancellationToken);

            if (!isUserFound)
            {
                return AppResult<UserTokenEntity>.Failed("UserId in this token is not found.");
            }

            var expiredDateTime = DateTimeOffset.FromUnixTimeSeconds(expInSeconds).DateTime;
            var userAccessToken = new UserTokenEntity
            {
                Id = tokenId,
                UserId = userId,
                ExpiredAt = expiredDateTime,
            };

            return AppResult<UserTokenEntity>.Success(userAccessToken);
        }

        public async Task<AppResult<UserTokenEntity>> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken)
        {
            var validationResult = await InternalValidateTokenAsync(
                token: refreshToken,
                tokenPurpose: TokenPurpose.RefreshToken,
                validateLifeTime: true);

            if (!validationResult.IsValid)
            {
                return AppResult<UserTokenEntity>.Failed(AppResult.DefaultMessage.InvalidRefreshToken);
            }

            var jtiClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(JwtRegisteredClaimNames.Jti));

            var userIdClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(UserCustomClaimTypes.UserId));

            if (Equals(userIdClaim, null) || Equals(jtiClaim, null))
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId claim has invalid format.");
            }

            var isValidUserId = Guid.TryParse(userIdClaim.Value, out Guid userId);
            var isValidTokenId = Guid.TryParse(jtiClaim.Value, out Guid tokenId);

            if (!isValidUserId || !isValidTokenId)
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId claim has invalid format.");
            }

            // Check if the tokenId that encapsulated in this token is existed or not.
            var isTokenFound = await _unitOfWork.UserTokenRepository.IsFoundByExpressionAsync(
                findExpresison: token => token.Id == tokenId,
                cancellationToken: cancellationToken);

            if (!isTokenFound)
            {
                return AppResult<UserTokenEntity>.Failed("TokenId is not found.");
            }

            var userRefreshToken = new UserTokenEntity
            {
                Id = tokenId,
                UserId = userId,
            };

            return AppResult<UserTokenEntity>.Success(userRefreshToken);
        }

        public async Task<AppResult<UserTokenEntity>> ValidateRegisterConfirmationTokenAsync(
            string confirmationToken,
            CancellationToken cancellationToken)
        {
            var validationResult = await InternalValidateTokenAsync(
                token: confirmationToken,
                tokenPurpose: TokenPurpose.RegisterConfirmationToken,
                validateLifeTime: true);

            if (!validationResult.IsValid)
            {
                return AppResult<UserTokenEntity>.Failed(AppResult.DefaultMessage.InvalidToken);
            }

            var userIdClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(UserCustomClaimTypes.UserId));

            var jtiClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(JwtRegisteredClaimNames.Jti));

            if (Equals(userIdClaim, null) || Equals(jtiClaim, null))
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId claim has invalid format.");
            }

            var isValidUserId = Guid.TryParse(userIdClaim.Value, out Guid userId);
            var isValidTokenId = Guid.TryParse(jtiClaim.Value, out Guid tokenId);

            if (!isValidUserId || !isValidTokenId)
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId claim has invalid format.");
            }

            // Check if the tokenId that encapsulated in this token is existed or not.
            var isTokenFound = await _unitOfWork.UserTokenRepository.IsFoundByExpressionAsync(
                findExpresison: token => token.Id == tokenId,
                cancellationToken: cancellationToken);

            if (!isTokenFound)
            {
                return AppResult<UserTokenEntity>.Failed("TokenId is not found.");
            }

            var userToken = new UserTokenEntity
            {
                Id = tokenId,
                UserId = userId,
                User = new UserEntity { Id = userId }
            };

            return AppResult<UserTokenEntity>.Success(userToken);
        }

        public async Task<AppResult<UserTokenEntity>> ValidateResetPasswordTokenAsync(
            string resetPasswordToken,
            CancellationToken cancellationToken)
        {
            var validationResult = await InternalValidateTokenAsync(
                token: resetPasswordToken,
                tokenPurpose: TokenPurpose.ResetPasswordToken,
                validateLifeTime: true);

            if (!validationResult.IsValid)
            {
                return AppResult<UserTokenEntity>.Failed(AppResult.DefaultMessage.InvalidToken);
            }

            var userIdClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(UserCustomClaimTypes.UserId));

            var jtiClaim = validationResult.ClaimsIdentity.FindFirst(
                match: claim => claim.Type.Equals(JwtRegisteredClaimNames.Jti));

            if (Equals(userIdClaim, null) || Equals(jtiClaim, null))
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId claim has invalid format.");
            }

            var isValidUserId = Guid.TryParse(userIdClaim.Value, out Guid userId);
            var isValidTokenId = Guid.TryParse(jtiClaim.Value, out Guid tokenId);

            if (!isValidUserId || !isValidTokenId)
            {
                return AppResult<UserTokenEntity>.Failed("UserId or TokenId claim has invalid format.");
            }

            // Check if the tokenId that encapsulated in this token is existed or not.
            var isTokenFound = await _unitOfWork.UserTokenRepository.IsFoundByExpressionAsync(
                findExpresison: token => token.Id == tokenId,
                cancellationToken: cancellationToken);

            if (!isTokenFound)
            {
                return AppResult<UserTokenEntity>.Failed("TokenId is not found.");
            }

            var userToken = new UserTokenEntity
            {
                Id = tokenId,
                UserId = userId,
                User = new UserEntity { Id = userId }
            };

            return AppResult<UserTokenEntity>.Success(userToken);
        }

        public UserTokenEntity CreateUserTokenByPurpose(
            Guid tokenId,
            UserEntity user,
            TokenPurpose tokenPurpose)
        {
            // Using Utc.Now for best accuracy to store in the database.
            var dateTimeUtcNow = DateTime.UtcNow;

            var userToken = new UserTokenEntity
            {
                Id = tokenId,
                UserId = user.Id,
                LoginProvider = _accessTokenOptions.Issuer,
                Value = tokenId.ToString(),
                CreatedAt = dateTimeUtcNow,
            };

            switch (tokenPurpose)
            {
                case TokenPurpose.RefreshToken:
                    userToken.Name = TokenNames.RefreshToken;
                    userToken.ExpiredAt = dateTimeUtcNow.AddDays(_refreshTokenOptions.LiveDays);

                    break;

                case TokenPurpose.RegisterConfirmationToken:
                    userToken.Name = TokenNames.RegisterConfirmationToken;
                    userToken.ExpiredAt = dateTimeUtcNow.AddMinutes(_registerConfirmationTokenOptions.LiveMinutes);

                    break;

                case TokenPurpose.ResetPasswordToken:
                    userToken.Name = TokenNames.ResetPasswordToken;
                    userToken.ExpiredAt = dateTimeUtcNow.AddMinutes(_registerConfirmationTokenOptions.LiveMinutes);

                    break;
            }

            userToken.Name = $"{userToken.Name}.{tokenId}";

            return userToken;
        }

        public async Task<bool> RemoveUserTokenByIdAsync(
            Guid tokenId,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(operation: async () =>
            {
                await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                try
                {
                    await _unitOfWork.UserTokenRepository.BulkDeleteByTokenIdAsync(
                        tokenId: tokenId,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.SaveChangesToDatabaseAsync(
                        cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(
                        cancellationToken: cancellationToken);

                    result = true;
                }
                catch
                {
                    await _unitOfWork.RollBackTransactionAsync(
                        cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(
                        cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public async Task<bool> RemoveAllTokensByTokenPurposeAsync(
            Guid userId,
            TokenPurpose tokenPurpose,
            CancellationToken cancellationToken)
        {
            if (Equals(tokenPurpose, null))
            {
                return false;
            }

            string tokenName = string.Empty;

            switch (tokenPurpose)
            {
                case TokenPurpose.AccessToken:
                    tokenName = TokenNames.RefreshToken;
                    break;

                case TokenPurpose.RefreshToken:
                    tokenName = TokenNames.RefreshToken;
                    break;

                case TokenPurpose.RegisterConfirmationToken:
                    tokenName = TokenNames.RegisterConfirmationToken;
                    break;

                case TokenPurpose.ResetPasswordToken:
                    tokenName = TokenNames.ResetPasswordToken;
                    break;
            }

            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(operation: async () =>
            {
                await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                try
                {
                    await _unitOfWork.UserTokenRepository.BulkDeleteAllTokensByUserIdAndTokenNameAsync(
                        userId: userId,
                        tokenName: tokenName,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(
                        cancellationToken: cancellationToken);

                    result = true;
                }
                catch
                {
                    await _unitOfWork.RollBackTransactionAsync(
                        cancellationToken: cancellationToken
                    );
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(
                        cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        #region Private Methods
        private SecurityTokenDescriptor InitTokenDescriptorByTokenPurpose(
            IEnumerable<Claim> claims,
            TimeSpan lifeSpan,
            TokenPurpose tokenPurpose)
        {
            SymmetricSecurityKey symmetricSecurityKey = GetSymmetricSecurityKeyByTokenPurpose(tokenPurpose);

            return new SecurityTokenDescriptor
            {
                Issuer = _accessTokenOptions.Issuer,
                Audience = _accessTokenOptions.Audience,
                Subject = new ClaimsIdentity(claims: claims),
                SigningCredentials = new SigningCredentials(
                    key: symmetricSecurityKey,
                    algorithm: SecurityAlgorithms.HmacSha256
                ),
                Expires = DateTime.Now.Add(lifeSpan)
            };
        }

        private SymmetricSecurityKey GetSymmetricSecurityKeyByTokenPurpose(TokenPurpose tokenPurpose)
        {
            SymmetricSecurityKey? symmetricSecurityKey;

            switch (tokenPurpose)
            {
                case TokenPurpose.AccessToken:
                    symmetricSecurityKey = _accessTokenOptions.GetSecurityKey();

                    break;

                case TokenPurpose.RefreshToken:
                    symmetricSecurityKey = _refreshTokenOptions.GetSecurityKey();

                    break;

                case TokenPurpose.RegisterConfirmationToken:
                    symmetricSecurityKey = _registerConfirmationTokenOptions.GetSecurityKey();

                    break;

                case TokenPurpose.ResetPasswordToken:
                    symmetricSecurityKey = _resetPasswordTokenOptions.GetSecurityKey();

                    break;

                default:
                    throw new ArgumentNullException(nameof(tokenPurpose));
            }

            return symmetricSecurityKey;
        }

        private TokenValidationParameters GetTokenValidationParametersByPurpose(
            TokenPurpose tokenPurpose,
            bool validateLifeTime = true)
        {
            SymmetricSecurityKey? symmetricSecurityKey = GetSymmetricSecurityKeyByTokenPurpose(tokenPurpose);

            return new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = validateLifeTime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _accessTokenOptions.Issuer,
                ValidAudience = _accessTokenOptions.Audience,
                IssuerSigningKey = symmetricSecurityKey
            };
        }

        /// <summary>
        ///     Validate the input <paramref name="token"/> is valid to this application's
        ///     requirements or not based on the input <paramref name="tokenPurpose"/>.
        /// </summary>
        /// <remarks>
        ///     The validation operation can be specified to validate the life time of
        ///     the token by using the <paramref name="validateLifeTime"/>.
        /// </remarks>
        /// <param name="token">
        ///     The token that need to be validated.
        /// </param>
        /// <param name="tokenPurpose">
        ///     The purpose of the input token that need to validate.
        /// </param>
        /// <param name="validateLifeTime">
        ///     To specify if the validation operation need to validate the life time of the token or not.
        /// </param>
        /// <returns></returns>
        private async Task<TokenValidationResult> InternalValidateTokenAsync(
            string token,
            TokenPurpose tokenPurpose,
            bool validateLifeTime = true)
        {
            try
            {
                // Validate the token credentials.
                var validationResult = await _securityTokenHandler.ValidateTokenAsync(
                    token: token,
                    validationParameters: GetTokenValidationParametersByPurpose(
                        tokenPurpose: tokenPurpose,
                        validateLifeTime: validateLifeTime));

                return validationResult;
            }
            catch (Exception)
            {
                var failedResult = new TokenValidationResult();
                failedResult.IsValid = false;

                return failedResult;
            }
        }
        #endregion
    }
}