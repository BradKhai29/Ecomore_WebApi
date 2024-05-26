using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface IProductImageRepository :
        IGenericRepository<ProductImageEntity>
    {
        Task<int> BulkUpdateDetailAsync(
            ProductImageEntity productImageToUpdate,
            CancellationToken cancellationToken);

        Task<int> BulkDeleteByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<int> GetNumberOfProductImagesByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken);
    }
}
