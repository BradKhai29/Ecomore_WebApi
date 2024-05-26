using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class OrderRepository :
        GenericRepository<OrderEntity>,
        IOrderRepository
    {
        public OrderRepository(DbContext dbContext) : base(dbContext)
        {
        }

        public Task<int> BulkUpdateOrderStatusAsync(
            OrderEntity orderToUpdate,
            CancellationToken cancellationToken)
        {
            var updateDateTime = DateTime.UtcNow;

            return _dbSet
                .Where(predicate: order => order.Id == orderToUpdate.Id)
                .ExecuteUpdateAsync(setPropertyCalls: order => order
                    .SetProperty(
                        order => order.StatusId,
                        order => orderToUpdate.StatusId)
                    .SetProperty(
                        order => order.UpdatedAt,
                        order => updateDateTime),
                    cancellationToken: cancellationToken);
        }

        public Task<OrderEntity> FindOrderByOrderCodeAsync(
            long orderCode,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(predicate: order => order.OrderCode == orderCode)
                .Select(order => new OrderEntity
                {
                    Id = order.Id,
                    OrderCode = order.OrderCode,
                    OrderItems = order.OrderItems.Select(orderItem => new OrderItemEntity
                    {
                        OrderId = orderItem.OrderId,
                        ProductId = orderItem.ProductId,
                        SellingQuantity = orderItem.SellingQuantity
                    })
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}
