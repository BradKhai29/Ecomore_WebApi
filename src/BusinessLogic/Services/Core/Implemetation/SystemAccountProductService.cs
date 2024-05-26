using BusinessLogic.Services.Core.Base;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class SystemAccountProductService : 
        ISystemAccountProductService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public SystemAccountProductService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<ProductEntity>> GetAllProductsAsync(
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.GetAllAsync(
                cancellationToken: cancellationToken);
        }

        public Task<bool> IsProductExistedByIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.IsFoundByExpressionAsync(
                findExpresison: product => product.Id == productId,
                cancellationToken: cancellationToken);
        }

        public Task<bool> CheckProductHasAnyOrderByIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.OrderItemRepository.IsFoundByExpressionAsync(
                findExpresison: orderItem => orderItem.ProductId == productId,
                cancellationToken: cancellationToken);
        }

        public async Task<bool> CreateNewProductAsync(
            ProductEntity product,
            IEnumerable<ProductImageEntity> productImages,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.ProductRepository.AddAsync(
                        newEntity: product,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.ProductImageRepository.AddRangeAsync(
                        newEntities: productImages,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.SaveChangesToDatabaseAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public Task<ProductEntity> FindProductByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.FindByIdForDetailDisplayAsync(
                productId: productId,
                cancellationToken: cancellationToken);
        }

        public Task<ProductEntity> FindProductByIdForUpdateAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.FindByIdForUpdateAsync(
                productId: productId,
                cancellationToken: cancellationToken);
        }

        public Task<int> CountProductImagesByIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductImageRepository.GetNumberOfProductImagesByProductIdAsync(
                productId: productId,
                cancellationToken: cancellationToken);
        }

        public async Task<bool> UpdateProductDetailAsync(
            ProductEntity productToUpdate,
            IList<ProductImageEntity> listOfUpdatedImages,
            IList<Guid> listOfImageIdToDelete, CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.ProductRepository.BulkUpdateProductDetailAsync(
                        productToUpdate: productToUpdate,
                        cancellationToken: cancellationToken);

                    // Delete the image if list is not null or empty.
                    if (!Equals(listOfImageIdToDelete, null) && listOfImageIdToDelete.Count > 0)
                    {
                        foreach (var deleteImageId in listOfImageIdToDelete)
                        {
                            await _unitOfWork.ProductImageRepository.BulkDeleteByIdAsync(
                                id: deleteImageId,
                                cancellationToken: cancellationToken);
                        }
                    }

                    // Update the image if the list is not null or empty.
                    if (!Equals(listOfUpdatedImages, null) && listOfUpdatedImages.Count > 0)
                    {
                        foreach(var imageToUpdate in listOfUpdatedImages)
                        {
                            await _unitOfWork.ProductImageRepository.BulkUpdateDetailAsync(
                                productImageToUpdate: imageToUpdate,
                                cancellationToken: cancellationToken);
                        }
                    }

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public async Task<bool> AddProductImagesAsync(
            IEnumerable<ProductImageEntity> productImages,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.ProductImageRepository.AddRangeAsync(
                        newEntities: productImages,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.SaveChangesToDatabaseAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public async Task<bool> DeleteProductByIdAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.ProductRepository.BulkDeleteByIdAsync(
                        productId: productId,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }
    }
}
