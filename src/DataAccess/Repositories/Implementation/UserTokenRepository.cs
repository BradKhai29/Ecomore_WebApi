using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class UserTokenRepository :
        GenericRepository<UserTokenEntity>,
        IUserTokenRepository
    {
        public UserTokenRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<int> BulkDeleteAllTokensByUserIdAndTokenNameAsync(
            Guid userId,
            string tokenName,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(
                    token => token.UserId == userId
                    && token.Name.Equals(tokenName))
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public Task<int> BulkDeleteByTokenIdAsync(Guid tokenId, CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(token => token.Id == tokenId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public Task<UserTokenEntity> FindByTokenIdAsync(Guid tokenId, CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(token => token.Id == tokenId)
                .Select(token => new UserTokenEntity
                {
                    Id = token.Id,
                    UserId = token.UserId,
                    ExpiredAt = token.ExpiredAt,
                    CreatedAt = token.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task<bool> IsStillValidByTokenIdAsync(Guid tokenId, CancellationToken cancellationToken)
        {
            var dateTimeNow = DateTime.Now;

            return _dbSet.AnyAsync(
                predicate: token => token.Id == tokenId
                && token.ExpiredAt > dateTimeNow);
        }
    }
}
