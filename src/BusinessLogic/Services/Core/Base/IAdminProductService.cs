using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface IAdminProductService
    {
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken);
    }
}
