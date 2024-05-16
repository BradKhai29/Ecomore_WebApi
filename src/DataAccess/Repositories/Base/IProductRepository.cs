using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface IProductRepository : IGenericRepository<ProductEntity>
    {
        /// <summary>
        ///     Get all products from the database without any pagination method.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The Task result that contains the list of products from the database.
        /// </returns>
        Task<IEnumerable<ProductEntity>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Find the product with the input productId.
        /// </summary>
        /// <remarks>
        ///     This operation will include AsNoTracking() method to avoid unecessary
        ///     tracking overhead.
        /// </remarks>
        /// <param name="productId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ProductEntity> FindByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);
    }
}
