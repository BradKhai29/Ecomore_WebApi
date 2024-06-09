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

        public Task<OrderEntity> FindOrderByIdForDetailDisplayAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(order => order.Id == orderId)
                .Select(order => new OrderEntity
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    OrderCode = order.OrderCode,
                    StatusId = order.StatusId,
                    TotalPrice = order.TotalPrice,
                    OrderGuestDetail = new OrderGuestDetailEntity
                    {
                        Email = order.OrderGuestDetail.Email,
                        GuestName = order.OrderGuestDetail.GuestName,
                        PhoneNumber = order.OrderGuestDetail.PhoneNumber,
                    },
                    OrderItems = order.OrderItems.Select(orderItem => new OrderItemEntity
                    {
                        ProductId = orderItem.ProductId,
                        SellingPrice = orderItem.SellingPrice,
                        SellingQuantity = orderItem.SellingQuantity,
                    })
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
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
                    GuestId = order.GuestId,
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

        public async Task<IEnumerable<OrderEntity>> GetAllOrdersByGuestIdAsync(
            Guid guestId,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(order => order.GuestId == guestId)
                .Select(order => new OrderEntity
                {
                    Id = order.Id,
                    OrderCode = order.OrderCode,
                    GuestId = guestId,
                    TotalPrice = order.TotalPrice,
                    StatusId = order.StatusId,
                    CreatedAt = order.CreatedAt,
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<OrderEntity>> GetAllOrdersByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(order => order.UserId == userId)
                .Select(order => new OrderEntity
                {
                    Id = order.Id,
                    OrderCode = order.OrderCode,
                    UserId = userId,
                    TotalPrice = order.TotalPrice,
                    StatusId = order.StatusId,
                    CreatedAt = order.CreatedAt,
                })
                .ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
