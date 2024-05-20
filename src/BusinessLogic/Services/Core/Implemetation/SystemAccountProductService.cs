using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class SystemAccountProductService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public SystemAccountProductService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.GetAllAsync(cancellationToken: cancellationToken);
        }
    }
}
