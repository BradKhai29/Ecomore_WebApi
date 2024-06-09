using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Base.Generics;

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly DbSet<TEntity> _dbSet;

    protected GenericRepository(DbContext dbContext)
    {
        _dbSet = dbContext.Set<TEntity>();
    }

    public ValueTask<EntityEntry<TEntity>> AddAsync(
        TEntity newEntity,
        CancellationToken cancellationToken)
    {
        return _dbSet.AddAsync(
            entity: newEntity,
            cancellationToken: cancellationToken);
    }

    public Task AddRangeAsync(
        IEnumerable<TEntity> newEntities,
        CancellationToken cancellationToken)
    {
        return _dbSet.AddRangeAsync(
            entities: newEntities,
            cancellationToken: cancellationToken);
    }

    public Task<bool> IsFoundByExpressionAsync(
        Expression<Func<TEntity, bool>> findExpresison,
        CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(findExpresison);
    }

    public void Remove(TEntity foundEntity)
    {
        _dbSet.Remove(foundEntity);
    }

    public void Update(TEntity foundEntity)
    {
        _dbSet.Update(foundEntity);
    }
}
