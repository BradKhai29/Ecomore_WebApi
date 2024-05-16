using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Base.Generics;

/// <summary>
///     The base interface for all Repository classes
///     that uses IdentityServer as a base to inherit from.
/// </summary>
/// <remarks>
///     This interface does not provide update method. Because the update
///     will be handled by bulk-update for each non-generic repository.
/// </remarks>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKeyId">
///     The data-type that used by entityId.
/// </typeparam>
public interface IIdentityRepository<TEntity, TKeyId>
    where TEntity : class, IEntity
{
    Task<IdentityResult> AddAsync(TEntity newEntity);

    Task<IdentityResult> UpdateAsync(TEntity foundEntity);

    Task<IdentityResult> RemoveAsync(TKeyId id);

    Task<TEntity> FindByIdAsync(TKeyId id);

    Task<TEntity> FindByNameAsync(string name);

    Task<bool> IsFoundByExpressionAsync(
        Expression<Func<TEntity, bool>> expression,
        CancellationToken cancellationToken);
}
