using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class SystemAccountRepository :
        GenericRepository<SystemAccountEntity>,
        ISystemAccountRepository
    {
        public SystemAccountRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<SystemAccountEntity> FindByEmailForLoginAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(systemAccount => systemAccount.Email.Equals(email))
                .Select(systemAccount => new SystemAccountEntity
                {
                    Id = systemAccount.Id,
                    Email = systemAccount.Email,
                    AccessFailedCount = systemAccount.AccessFailedCount,
                    PasswordHash = systemAccount.PasswordHash,
                    LockoutEnd = systemAccount.LockoutEnd,
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdateSystemAccountForFailedAccessAsync(
            SystemAccountEntity systemAccountEntity,
            CancellationToken cancellationToken)
        {
            var accessFailedCount = systemAccountEntity.AccessFailedCount;
            var lockoutEnd = systemAccountEntity.LockoutEnd;

            return _dbSet
                .Where(systemAccount => systemAccount.Id.Equals(systemAccountEntity.Id))
                .ExecuteUpdateAsync(
                    systemAccount => systemAccount
                        .SetProperty(
                            systemAccount => systemAccount.AccessFailedCount,
                            systemAccount => accessFailedCount)
                        .SetProperty(
                            systemAccount => systemAccount.LockoutEnd,
                            systemAccount => lockoutEnd),
                    cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdatePasswordByAccountIdAsync(
            Guid systemAccountId,
            string passwordHash,
            CancellationToken cancellationToken)
        {
            var updatedAt = DateTime.UtcNow;

            return _dbSet
                .Where(systemAccount => systemAccount.Id.Equals(systemAccountId))
                .ExecuteUpdateAsync(
                    systemAccount => systemAccount
                        .SetProperty(
                            systemAccount => systemAccount.PasswordHash,
                            systemAccount => passwordHash)
                        .SetProperty(
                            systemAccount => systemAccount.UpdatedAt,
                            systemAccount => updatedAt),
                    cancellationToken: cancellationToken);
        }
    }
}
