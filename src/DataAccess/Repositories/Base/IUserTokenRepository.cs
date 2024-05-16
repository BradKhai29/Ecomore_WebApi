using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface IUserTokenRepository :
        IGenericRepository<UserTokenEntity>
    {
        /// <summary>
        ///     Check if the target token is still valid (ExpiredTime is later than <see cref="DateTime.Now"/>) or not.
        /// </summary>
        /// <param name="tokenId">
        ///     The tokenId that used to find the token.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsStillValidByTokenIdAsync(Guid tokenId, CancellationToken cancellationToken);

        Task<int> BulkDeleteAllTokensByUserIdAndTokenNameAsync(
            Guid userId,
            string tokenName,
            CancellationToken cancellationToken);

        Task<int> BulkDeleteByTokenIdAsync(
            Guid tokenId,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Find the target userToken by the input <paramref name="tokenId"/>.
        /// </summary>
        /// <param name="tokenId">
        ///     The tokenId that will be used to find.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The userToken record from the database.
        /// </returns>
        Task<UserTokenEntity> FindByTokenIdAsync(
            Guid tokenId,
            CancellationToken cancellationToken);
    }
}
