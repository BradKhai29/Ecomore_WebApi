using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Base.Generics;

public abstract class IdentityRepository<TEntity> : IIdentityRepository<TEntity, Guid>
    where TEntity : class, IEntity
{
    protected readonly DbSet<TEntity> _dbSet;

    protected IdentityRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
    }

    public abstract Task<IdentityResult> AddAsync(TEntity newEntity);

    public abstract Task<IdentityResult> RemoveAsync(Guid id);

    public abstract Task<IdentityResult> UpdateAsync(TEntity foundEntity);

    public abstract Task<TEntity> FindByIdAsync(Guid id);

    public abstract Task<TEntity> FindByNameAsync(string name);

    public Task<bool> IsFoundByExpressionAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(
            predicate: expression,
            cancellationToken: cancellationToken);
    }
}
