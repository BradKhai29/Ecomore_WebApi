using BusinessLogic.Models;
using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface IUserProductService
    {
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync(
            int pageSize,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> GetRandomProductsAsync(
            int randomItemCount,
            CancellationToken cancellationToken);

        Task<ProductEntity> FindProductByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);

        Task<AppResult<ProductEntity>> FindProductByIdForAddingToCartAsync(
            Guid productId,
            CancellationToken cancellationToken); 

        /// <summary>
        ///     Get all the products that has id contained in the
        ///     specified <paramref name="productIdList"/>.
        /// </summary>
        /// <param name="productIdList">
        ///     The list of productId that need to get from the database
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<ProductEntity>> GetAllProductsFromIdListForDisplayShoppingCartAsync(
            IEnumerable<Guid> productIdList,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> GetAllProductsFromIdListForDisplayOrderAsync(
            IEnumerable<Guid> productIdList,
            CancellationToken cancellationToken);
    }
}
