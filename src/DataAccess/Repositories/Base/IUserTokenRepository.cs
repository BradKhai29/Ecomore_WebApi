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
    }
}
