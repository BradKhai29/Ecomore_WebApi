using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ISystemAccountProductService
    {
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken);
    }
}
