using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;

namespace DataAccess.Repositories.Base
{
    public interface IOrderRepository :
        IGenericRepository<OrderEntity>
    {
        Task<OrderEntity> FindOrderByOrderCodeAsync(
            long orderCode,
            CancellationToken cancellationToken);

        Task<int> BulkUpdateOrderStatusAsync(
            OrderEntity orderToUpdate,
            CancellationToken cancellationToken);
    }
}
