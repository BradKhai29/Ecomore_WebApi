using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface IProductRepository : 
        IGenericRepository<ProductEntity>
    {
        /// <summary>
        ///     Get all products from the database without any pagination method.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The Task result that contains the list of products from the database.
        /// </returns>
        Task<IEnumerable<ProductEntity>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> GetAllAsync(
            int pageSize,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Find the product with the input productId.
        /// </summary>
        /// <remarks>
        ///     This operation will include AsNoTracking() method 
        ///     to avoid unnecessary tracking overhead.
        /// </remarks>
        /// <param name="productId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ProductEntity> FindByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<ProductEntity> FindByIdForAddingToCartAsync(
            Guid productId,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Get the list of products which's id is specified in the <paramref name="productIds"/> list.
        ///     This product list will be used to handle the order confirmation operation.
        /// </summary>
        /// <param name="productIds">The list of related productId.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<ProductEntity>> FindAllProductsByIdListForOrderConfirmAsync(
            IEnumerable<Guid> productIds,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> FindAllProductsByIdListForDisplayOrderAsync(
            IEnumerable<Guid> productIds,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> FindAllProductsByIdListAsync(
            IEnumerable<Guid> productIds,
            CancellationToken cancellationToken);

        Task<ProductEntity> FindByIdForDisplayShoppingCartAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);

        Task<IEnumerable<Guid>> GetAllProductIdsByCategoryIdsAsync(
            IEnumerable<Guid> categoryIds,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Find the specified product by the input <paramref name="productId"/>
        ///     to display the information for updating operation.
        /// </summary>
        /// <remarks>
        ///     This operation will include AsNoTracking() method 
        ///     to avoid unnecessary tracking overhead.
        /// </remarks>
        /// <param name="productId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ProductEntity> FindByIdForUpdateAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<int> BulkUpdateProductDetailAsync(
            ProductEntity productToUpdate,
            CancellationToken cancellationToken);

        Task<int> BulkDeleteByIdAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<int> BulkUpdateQuantityInStockAsync(
            ProductEntity productToUpdate,
            CancellationToken cancellationToken);
    }
}
