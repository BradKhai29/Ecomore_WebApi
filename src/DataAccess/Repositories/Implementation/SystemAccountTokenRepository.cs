using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class SystemAccountTokenRepository :
        GenericRepository<SystemAccountTokenEntity>,
        ISystemAccountTokenRepository
    {
        public SystemAccountTokenRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<int> BulkDeleteByTokenIdAsync(
            Guid tokenId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(token => token.Id == tokenId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }
    }
}
