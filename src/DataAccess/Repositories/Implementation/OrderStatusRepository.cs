using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class OrderStatusRepository :
        GenericRepository<OrderStatusEntity>,
        IOrderStatusRepository
    {
        public OrderStatusRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
