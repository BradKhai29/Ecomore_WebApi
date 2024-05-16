using BusinessLogic.Models;
using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface IUserProductService
    {
        Task<IEnumerable<ProductEntity>> GetAllProductsAsync(CancellationToken cancellationToken);

        Task<ProductEntity> FindProductByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken);

        Task<IEnumerable<ProductEntity>> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken);
    }
}
