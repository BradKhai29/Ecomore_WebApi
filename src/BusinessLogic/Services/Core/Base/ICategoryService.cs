using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryEntity>> GetAllCategoriesAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Check if the provided <paramref name="categoryId"/>
        ///     has products belonged to it or not.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CheckCategoryHasProductByIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);

        Task<bool> IsCategoryExistedByNameAsync(string categoryName, CancellationToken cancellationToken);
        
        Task<bool> IsCategoryExistedByIdAsync(Guid categoryId, CancellationToken cancellationToken);

        Task<bool> AddCategoryAsync(CategoryEntity category, CancellationToken cancellationToken);

        Task<bool> DeleteCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken);

        Task<bool> UpdateCategoryAsync(CategoryEntity category, CancellationToken cancellationToken);
    }
}
