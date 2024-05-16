using BusinessLogic.Enums;
using BusinessLogic.Models;
using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface IUserTokenService
    {
        /// <summary>
        ///     Generate a jwt-format access-token from the received credentials
        ///     and belonged to the input <paramref name="tokenId"/>.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, Iat, UserId, FullName, AvatarUrl.
        /// </remarks>
        /// <param name="user">
        ///     The user's claims this access token will encapsulate.
        /// </param>
        /// <returns>
        ///     A string that represents the access-token value.
        /// </returns>
        string GenerateAccessToken(
            UserEntity user,
            Guid tokenId);

        /// <summary>
        ///     Generate a jwt-format refresh-token from the received credentials.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, Iat, UserId.
        ///     <br/><br/>
        ///     The life span of this token will be configured through 
        ///     the <see cref="Options.Models.Jwts.UserAccount.RefreshTokenOptions"/>         
        ///     in appsettings.json file.
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="tokenId"></param>
        /// <returns>
        ///     A string that represents the refresh-token value.
        /// </returns>
        string GenerateRefreshToken(
            UserEntity user,
            Guid tokenId);

        /// <summary>
        ///     Generate a jwt-format register confirmation token
        ///     from the credentials of the input <paramref name="user"/>.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, UserId.
        ///     <br/><br/>
        ///     The life span of this token will be configured through 
        ///     the <see cref="Options.Models.Jwts.UserAccount.RegisterConfirmationTokenOptions"/>         
        ///     in appsettings.json file.
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="tokenId"></param>
        /// <returns>
        ///     A string that represent the token.
        /// </returns>
        string GenerateRegisterConfirmationToken(
            UserEntity user,
            Guid tokenId);

        /// <summary>
        ///     Generate a jwt-format reset password token
        ///     from the credentials of the input <paramref name="user"/>.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, UserId.
        ///     <br/><br/>
        ///     The life span of this token will be configured through 
        ///     the <see cref="Options.Models.Jwts.UserAccount.ResetPasswordTokenOptions"/>         
        ///     in appsettings.json file.
        /// </remarks>
        /// <param name="user"></param>
        /// <param name="tokenId"></param>
        /// <returns>
        ///     A string that represent the token.
        /// </returns>
        string GenerateResetPasswordToken(
            UserEntity user,
            Guid tokenId);

        /// <summary>
        ///     Create a new <see cref="UserTokenEntity"/> instance from 
        ///     the provided credentials
        /// </summary>
        /// <param name="tokenId">
        ///     The tokenId of this token instance.
        /// </param>
        /// <param name="user">
        ///     The user that possess this token.
        /// </param>
        /// <param name="tokenPurpose">
        ///     The purpose of this token.
        /// </param>
        /// <returns>
        ///     A new <see cref="UserTokenEntity"/> instance.
        /// </returns>
        UserTokenEntity CreateUserTokenByPurpose(
            Guid tokenId,
            UserEntity user,
            TokenPurpose tokenPurpose);

        Task<bool> SaveUserTokenAsync(
            UserTokenEntity userToken,
            CancellationToken cancellationToken);

        Task<bool> RemoveUserTokenByIdAsync(
            Guid tokenId,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Remove all tokens of the specific user with based on the input <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tokenPurpose"></param>
        /// <returns></returns>
        Task<bool> RemoveAllTokensByTokenPurposeAsync(
            Guid userId,
            TokenPurpose tokenPurpose,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Validate the input <paramref name="confirmationToken"/> 
        ///     is valid or not.
        /// </summary>
        /// <param name="confirmationToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     An <see cref="AppResult{TValue}"/> instance that contains the information
        ///     about the <paramref name="confirmationToken"/> after validation.
        /// </returns>
        Task<AppResult<UserTokenEntity>> ValidateRegisterConfirmationTokenAsync(
            string confirmationToken,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Validate the input <paramref name="accessToken"/> is valid or not 
        ///     without checking the expired time.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     An <see cref="AppResult{TValue}"/> instance that contains information
        ///     about the <paramref name="accessToken"/> after validation.
        /// </returns>
        Task<AppResult<UserTokenEntity>> ValidateAccessTokenAsync(
            string accessToken,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Validate the input <paramref name="refreshToken"/> is valid or not.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     An <see cref="AppResult{TValue}"/> instance that contains information
        ///     about the <paramref name="refreshToken"/> after validation.
        /// </returns>
        Task<AppResult<UserTokenEntity>> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Validate the input <paramref name="resetPasswordToken"/> is valid or not.
        /// </summary>
        /// <param name="resetPasswordToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     An <see cref="AppResult{TValue}"/> instance that contains information
        ///     about the <paramref name="resetPasswordToken"/> after validation.
        /// </returns>
        Task<AppResult<UserTokenEntity>> ValidateResetPasswordTokenAsync(
            string resetPasswordToken,
            CancellationToken cancellationToken);
    }
}
