using DataAccess.DataSeedings;
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

        public Task<int> BulkDeleteByIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(product => product.Id == productId)
                .ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdateProductDetailAsync(
            ProductEntity productToUpdate,
            CancellationToken cancellationToken)
        {
            var updateDateTime = productToUpdate.UpdatedAt;

            return _dbSet
                .Where(predicate: product => product.Id == productToUpdate.Id)
                .ExecuteUpdateAsync(
                    setPropertyCalls: product => product
                        .SetProperty(
                            product => product.Name,
                            product => productToUpdate.Name)
                        .SetProperty(
                            product => product.ProductStatusId,
                            product => productToUpdate.ProductStatusId)
                        .SetProperty(
                            product => product.CategoryId,
                            product => productToUpdate.CategoryId)
                        .SetProperty(
                            product => product.Description,
                            product => productToUpdate.Description)
                        .SetProperty(
                            product => product.UnitPrice,
                            product => productToUpdate.UnitPrice)
                        .SetProperty(
                            product => product.QuantityInStock,
                            product => productToUpdate.QuantityInStock)
                        .SetProperty(
                            product => product.UpdatedBy,
                            product => productToUpdate.UpdatedBy)
                        .SetProperty(
                            product => product.UpdatedAt,
                            product => updateDateTime),
                    cancellationToken: cancellationToken);
        }



        public async Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
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

        public Task<ProductEntity> FindByIdForAddingToCartAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(product => product.Id == productId)
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    QuantityInStock = product.QuantityInStock,
                    ProductStatusId = product.ProductStatusId
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task<ProductEntity> FindByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
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
                    CreatedAt = product.CreatedAt,
                    SellingCount = product.SellingCount,
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        StorageUrl = image.StorageUrl,
                    })
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task<ProductEntity> FindByIdForDisplayShoppingCartAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(product => product.Id == productId)
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ProductStatusId = product.ProductStatusId,
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        StorageUrl = image.StorageUrl,
                    })
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task<ProductEntity> FindByIdForUpdateAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
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
                        Id = product.ProductStatus.Id,
                        Name = product.ProductStatus.Name
                    },
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock,
                    CreatedAt = product.CreatedAt,
                    SellingCount = product.SellingCount,
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        Id = image.Id,
                        UploadOrder = image.UploadOrder,
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

        public async Task<IEnumerable<ProductEntity>> GetAllAsync(
            int pageSize,
            CancellationToken cancellationToken)
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
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> FindAllProductsByIdListForOrderConfirmAsync(
            IEnumerable<Guid> productIds,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(product => productIds.Contains(product.Id))
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    QuantityInStock = product.QuantityInStock
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdateQuantityInStockAsync(
            ProductEntity productToUpdate,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .Where(product => product.Id == productToUpdate.Id)
                .ExecuteUpdateAsync(product => product
                    .SetProperty(
                        product => product.QuantityInStock,
                        product => productToUpdate.QuantityInStock),
                    cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> FindAllProductsByIdListForDisplayOrderAsync(
            IEnumerable<Guid> productIdList,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(product => productIdList.Contains(product.Id))
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    Name = product.Name,
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<Guid>> GetAllProductIdsByCategoryIdsAsync(
            IEnumerable<Guid> categoryIds,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(product => categoryIds.Contains(product.CategoryId))
                .Select(selector: product => product.Id)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> FindAllProductsByIdListAsync(
            IEnumerable<Guid> productIds,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(
                    product => productIds.Contains(product.Id) 
                    && product.ProductStatusId == ProductStatuses.InStock.Id
                )
                .Select(product => new ProductEntity
                {
                    Id = product.Id,
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    Category = new CategoryEntity
                    {
                        Id = product.CategoryId,
                        Name = product.Category.Name
                    },
                    ProductImages = product.ProductImages.Select(image => new ProductImageEntity
                    {
                        StorageUrl = image.StorageUrl
                    })
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
