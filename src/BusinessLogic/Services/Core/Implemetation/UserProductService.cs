using BusinessLogic.Models;
using BusinessLogic.Services.Core.Base;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using System.Security.Cryptography;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class UserProductService : IUserProductService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public UserProductService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.FindAllProductsByCategoryIdAsync(
                categoryId: categoryId,
                cancellationToken: cancellationToken);
        }

        public async Task<AppResult<ProductEntity>> FindProductByIdForAddingToCartAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var foundProduct = await _unitOfWork.ProductRepository.FindByIdForAddingToCartAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (Equals(foundProduct, null))
            {
                return AppResult<ProductEntity>.Failed($"Product with Id [{productId}] is not found.");
            }

            var isInStock = foundProduct.QuantityInStock > 0
                && foundProduct.ProductStatusId == ProductStatuses.InStock.Id;

            if (!isInStock)
            {
                return AppResult<ProductEntity>.Failed("Product is out of stock or not available.");
            }

            return AppResult<ProductEntity>.Success(foundProduct);
        }

        public Task<ProductEntity> FindProductByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.FindByIdForDetailDisplayAsync(
                productId: productId,
                cancellationToken: cancellationToken);
        }

        public Task<IEnumerable<ProductEntity>> GetAllProductsAsync(
            int pageSize,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.GetAllAsync(
                pageSize: pageSize,
                cancellationToken: cancellationToken);
        }

        public Task<IEnumerable<ProductEntity>> GetAllProductsFromIdListForDisplayOrderAsync(
            IEnumerable<Guid> productIdList,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.FindAllProductsByIdListForDisplayOrderAsync(
                productIds: productIdList,
                cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> GetAllProductsFromIdListForDisplayShoppingCartAsync(
            IEnumerable<Guid> productIdList,
            CancellationToken cancellationToken)
        {
            var productList = new List<ProductEntity>(productIdList.Count());

            foreach (var productId in productIdList)
            {
                var foundProduct = await _unitOfWork.ProductRepository.FindByIdForDisplayShoppingCartAsync(
                    productId: productId,
                    cancellationToken: cancellationToken);

                if (
                    !Equals(foundProduct, null) 
                    && foundProduct.ProductStatusId == ProductStatuses.InStock.Id
                )
                {
                    productList.Add(foundProduct);
                }
            }

            return productList;
        }

        public async Task<IEnumerable<ProductEntity>> GetRandomProductsAsync(
            int randomItemCount,
            CancellationToken cancellationToken)
        {
            var randomCategoryIds = await _unitOfWork.CategoryRepository.GetRandomCategoryIdListAsync(
                randomCount: 2,
                cancellationToken: cancellationToken);

            var productIds = await _unitOfWork.ProductRepository.GetAllProductIdsByCategoryIdsAsync(
                categoryIds: randomCategoryIds,
                cancellationToken: cancellationToken);

            int totalProductCount = productIds.Count();
            if (randomItemCount >= totalProductCount)
            {
                return await _unitOfWork.ProductRepository.FindAllProductsByIdListAsync(
                    productIds: productIds,
                    cancellationToken: cancellationToken);
            }

            // Get random product from the productIds with
            // total number of item is equal to the input randomItemCount.
            var randomProductIds = new List<Guid>(randomItemCount);
            var leftProductIdsNotAddedToRandomList = new List<Guid>();

            int addedItemCount = 0;
            foreach (var productId in productIds)
            {
                // If random number is in range [6, 10],
                // then add that item to the random list.
                if (RandomNumberGenerator.GetInt32(1, 10) > 5)
                {
                    randomProductIds.Add(productId);
                    addedItemCount++;

                    if (addedItemCount == randomItemCount)
                    {
                        break;
                    }
                }
                else
                {
                    leftProductIdsNotAddedToRandomList.Add(productId);
                }
            }

            // If the randomCategoryIdList.length is not equal to the randomCount,
            // then add the left ids from the leftCategoryIdList to the list.
            if (addedItemCount < randomItemCount)
            {
                var leftProductIds = leftProductIdsNotAddedToRandomList.Take(randomItemCount - addedItemCount);
                randomProductIds.AddRange(leftProductIds);
            }

            leftProductIdsNotAddedToRandomList.TrimExcess();

            var randomProducts = await _unitOfWork.ProductRepository.FindAllProductsByIdListAsync(
                productIds: randomProductIds,
                cancellationToken: cancellationToken);

            return randomProducts;
        }
    }
}
