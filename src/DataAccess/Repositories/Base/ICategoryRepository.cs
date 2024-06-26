﻿using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface ICategoryRepository :
        IGenericRepository<CategoryEntity>
    {
        Task<IEnumerable<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Guid>> GetRandomCategoryIdListAsync(
            int randomCount,
            CancellationToken cancellationToken);

        Task<int> BulkDeleteByIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);

        Task<int> BulkUpdateAsync(
            CategoryEntity categoryToUpdate,
            CancellationToken cancellationToken);
    }
}
