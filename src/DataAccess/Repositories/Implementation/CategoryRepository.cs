using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

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

        public async Task<IEnumerable<Guid>> GetRandomCategoryIdListAsync(
            int totalRandomItemCount,
            CancellationToken cancellationToken)
        {
            var categoryIdList = await _dbSet
                .AsNoTracking()
                .Select(selector: category => category.Id)
                .ToListAsync(cancellationToken: cancellationToken);

            int totalNumberOfCategoryInDatabase = categoryIdList.Count;
            if (totalRandomItemCount >= totalNumberOfCategoryInDatabase)
            {
                return categoryIdList;
            }

            var randomCategoryIds = new List<Guid>(totalRandomItemCount);
            var leftCategoryIdsNotAddedToRandomList = new List<Guid>();

            int addedItemCount = 0;
            foreach (var categoryId in categoryIdList)
            {
                // If random number is in range [6, 10],
                // then add that item to the random list.
                if (RandomNumberGenerator.GetInt32(1, 10) > 5)
                {
                    randomCategoryIds.Add(categoryId);
                    addedItemCount++;

                    if (addedItemCount == totalRandomItemCount)
                    {
                        break;
                    }
                }
                else
                {
                    leftCategoryIdsNotAddedToRandomList.Add(categoryId);
                }
            }

            // If the randomCategoryIdList.length is not equal to the randomCount,
            // then add the left ids from the leftCategoryIdList to the list.
            if (addedItemCount < totalRandomItemCount)
            {
                var leftCategoryIds = leftCategoryIdsNotAddedToRandomList.Take(totalRandomItemCount - addedItemCount);
                randomCategoryIds.AddRange(leftCategoryIds);
            }

            categoryIdList.Clear();
            leftCategoryIdsNotAddedToRandomList.TrimExcess();
            
            return randomCategoryIds;
        }
    }
}
