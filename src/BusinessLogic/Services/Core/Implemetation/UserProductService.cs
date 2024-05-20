using BusinessLogic.Models;
using BusinessLogic.Services.Core.Base;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;

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

        public Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.GetAllAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<ProductEntity>> GetAllProductsFromIdListAsync(
            IEnumerable<Guid> productIdList,
            CancellationToken cancellationToken)
        {
            var productList = new List<ProductEntity>(productIdList.Count());

            foreach (var productId in productIdList)
            {
                var foundProduct = await _unitOfWork.ProductRepository.FindByIdForShoppingCartDisplayAsync(
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
    }
}
