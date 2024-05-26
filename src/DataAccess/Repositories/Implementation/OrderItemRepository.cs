using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class OrderItemRepository :
        GenericRepository<OrderItemEntity>,
        IOrderItemRepository
    {
        public OrderItemRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
