using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ISystemAccountProductService
    {
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync(
            CancellationToken cancellationToken);

        Task<int> CountProductImagesByIdAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<bool> IsProductExistedByIdAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<bool> CheckProductHasAnyOrderByIdAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<ProductEntity> FindProductByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<ProductEntity> FindProductByIdForUpdateAsync(
            Guid productId,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Create the new product and save to database.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="productImages"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CreateNewProductAsync(
            ProductEntity product,
            IEnumerable<ProductImageEntity> productImages,
            CancellationToken cancellationToken);

        Task<bool> AddProductImagesAsync(
            IEnumerable<ProductImageEntity> productImages,
            CancellationToken cancellationToken);

        Task<bool> UpdateProductDetailAsync(
            ProductEntity productToUpdate,
            IList<ProductImageEntity> listOfUpdatedImages,
            IList<Guid> listOfImageIdToDelete,
            CancellationToken cancellationToken);

        Task<bool> DeleteProductByIdAsync(
            Guid productId,
            CancellationToken cancellationToken);
    }
}
