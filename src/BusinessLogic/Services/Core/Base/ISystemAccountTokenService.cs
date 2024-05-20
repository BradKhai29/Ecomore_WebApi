using BusinessLogic.Enums;
using BusinessLogic.Models;
using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ISystemAccountTokenService
    {
        /// <summary>
        ///     Generate a jwt-format access-token from the received credentials
        ///     and belonged to the input <paramref name="tokenId"/>.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, Iat, SystemAccountId.
        /// </remarks>
        /// <param name="systemAccount">
        ///     The system account's claims this access token will encapsulate.
        /// </param>
        /// <returns>
        ///     A string that represents the access-token value.
        /// </returns>
        string GenerateAccessToken(
            SystemAccountEntity systemAccount,
            Guid tokenId);

        /// <summary>
        ///     Generate a jwt-format refresh-token from the received credentials.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, Iat, SystemAccountId.
        ///     <br/><br/>
        ///     The life span of this token will be configured through 
        ///     the <see cref="Options.Models.Jwts.SystemAccount.RefreshTokenOptions"/>         
        ///     in appsettings.json file.
        /// </remarks>
        /// <param name="systemAccount"></param>
        /// <param name="tokenId"></param>
        /// <returns>
        ///     A string that represents the refresh-token value.
        /// </returns>
        string GenerateRefreshToken(
            SystemAccountEntity systemAccount,
            Guid tokenId);

        /// <summary>
        ///     Generate a jwt-format register confirmation token
        ///     from the credentials of the input <paramref name="systemAccount"/>.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, SystemId.
        ///     <br/><br/>
        ///     The life span of this token will be configured through 
        ///     the <see cref="Options.Models.Jwts.SystemAccount.RegisterConfirmationTokenOptions"/>  
        ///     in appsettings.json file.
        /// </remarks>
        /// <param name="systemAccount"></param>
        /// <param name="tokenId"></param>
        /// <returns>
        ///     A string that represent the token.
        /// </returns>
        string GenerateRegisterConfirmationToken(
            SystemAccountEntity systemAccount,
            Guid tokenId);

        /// <summary>
        ///     Generate a jwt-format reset password token
        ///     from the credentials of the input <paramref name="systemAccount"/>.
        /// </summary>
        /// <remarks>
        ///     This token will encapsulte some claims, including: Jti, SystemAccountId.
        ///     <br/><br/>
        ///     The life span of this token will be configured through 
        ///     the <see cref="Options.Models.Jwts.SystemAccount.ResetPasswordTokenOptions"/>         
        ///     in appsettings.json file.
        /// </remarks>
        /// <param name="systemAccount"></param>
        /// <param name="tokenId"></param>
        /// <returns>
        ///     A string that represent the token.
        /// </returns>
        string GenerateResetPasswordToken(
            SystemAccountEntity systemAccount,
            Guid tokenId);

        /// <summary>
        ///     Create a new <see cref="SystemAccountTokenEntity"/> instance from 
        ///     the provided credentials. Any DateTime value will store
        ///     with <see cref="DateTimeKind.Utc"/>.
        /// </summary>
        /// <param name="tokenId">
        ///     The tokenId of this token instance.
        /// </param>
        /// <param name="systemAccount">
        ///     The user that possess this token.
        /// </param>
        /// <param name="tokenPurpose">
        ///     The purpose of this token.
        /// </param>
        /// <returns>
        ///     A new <see cref="SystemAccountTokenEntity"/> instance.
        /// </returns>
        SystemAccountTokenEntity CreateSystemAccountTokenByPurpose(
            Guid tokenId,
            SystemAccountEntity systemAccount,
            TokenPurpose tokenPurpose);

        Task<bool> SaveSystemAccountTokenAsync(
            SystemAccountTokenEntity systemAccountToken,
            CancellationToken cancellationToken);

        Task<bool> RemoveSystemAccountTokenByIdAsync(
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
        Task<AppResult<SystemAccountTokenEntity>> ValidateRegisterConfirmationTokenAsync(
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
        Task<AppResult<SystemAccountTokenEntity>> ValidateAccessTokenAsync(
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
        Task<AppResult<SystemAccountTokenEntity>> ValidateRefreshTokenAsync(
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
        Task<AppResult<SystemAccountTokenEntity>> ValidateResetPasswordTokenAsync(
            string resetPasswordToken,
            CancellationToken cancellationToken);
    }
}
