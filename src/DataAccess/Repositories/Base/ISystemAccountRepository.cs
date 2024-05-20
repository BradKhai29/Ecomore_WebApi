using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface ISystemAccountRepository :
        IGenericRepository<SystemAccountEntity>
    {
        Task<SystemAccountEntity> FindByEmailForLoginAsync(string email, CancellationToken cancellationToken);

        Task<int> BulkUpdateSystemAccountForFailedAccessAsync(
            SystemAccountEntity systemAccountEntity,
            CancellationToken cancellationToken);

        Task<int> BulkUpdatePasswordByAccountIdAsync(
            Guid systemAccountId,
            string passwordHash,
            CancellationToken cancellationToken);
    }
}
