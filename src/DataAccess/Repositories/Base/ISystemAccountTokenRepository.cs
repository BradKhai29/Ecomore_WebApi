using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface ISystemAccountTokenRepository :
        IGenericRepository<SystemAccountTokenEntity>
    {
        Task<int> BulkDeleteByTokenIdAsync(
            Guid tokenId,
            CancellationToken cancellationToken);
    }
}
