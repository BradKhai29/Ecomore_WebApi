using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class CategoryRepository :
        GenericRepository<CategoryEntity>,
        ICategoryRepository
    {
        public CategoryRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<int> BulkDeleteByIdAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(predicate: category => category.Id == categoryId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdateAsync(CategoryEntity categoryToUpdate, CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(predicate: category => category.Id == categoryToUpdate.Id)
                .ExecuteUpdateAsync(
                    setPropertyCalls: category => category
                        .SetProperty(
                            category => category.Name,
                            category => categoryToUpdate.Name),
                    cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Select(category => new CategoryEntity
                {
                    Id = category.Id,
                    Name = category.Name,
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
