using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class ProductImageRepository :
        GenericRepository<ProductImageEntity>,
        IProductImageRepository
    {
        public ProductImageRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<int> BulkDeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(image => image.Id == id)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdateDetailAsync(
            ProductImageEntity productImageToUpdate,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(image => image.Id == productImageToUpdate.Id)
                .ExecuteUpdateAsync(setPropertyCalls: image => image
                    .SetProperty(
                        image => image.FileName,
                        image => productImageToUpdate.FileName)
                    .SetProperty(
                        image => image.StorageUrl,
                        image => productImageToUpdate.StorageUrl),
                    cancellationToken: cancellationToken);
        }

        public Task<int> GetNumberOfProductImagesByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(productImage => productImage.ProductId == productId)
                .CountAsync(cancellationToken: cancellationToken);
        }
    }
}
