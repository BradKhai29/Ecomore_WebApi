using BusinessLogic.Services.Core.Base;
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
    }
}
