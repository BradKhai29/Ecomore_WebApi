using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class ProductRepository :
        GenericRepository<ProductEntity>,
        IProductRepository
    {
        public ProductRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .Where(product => product.CategoryId == categoryId)
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    Category = new CategoryEntity
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    },
                    Name = product.Name,
                    Description = product.Description,
                    ProductStatus = new ProductStatusEntity
                    {
                        Name = product.ProductStatus.Name
                    },
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        StorageUrl = image.StorageUrl,
                    })
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<ProductEntity> FindByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(product => product.Id == productId)
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    Category = new CategoryEntity
                    {
                        Id = product.Category.Id,
                        Name = product.Category.Name
                    },
                    Name = product.Name,
                    Description = product.Description,
                    ProductStatus = new ProductStatusEntity
                    {
                        Name = product.ProductStatus.Name
                    },
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        StorageUrl = image.StorageUrl,
                    })
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    Category = new CategoryEntity
                    {
                        Name = product.Category.Name
                    },
                    Name = product.Name,
                    Description = product.Description,
                    ProductStatus = new ProductStatusEntity
                    {
                        Name = product.ProductStatus.Name
                    },
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        StorageUrl = image.StorageUrl,
                    })
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
