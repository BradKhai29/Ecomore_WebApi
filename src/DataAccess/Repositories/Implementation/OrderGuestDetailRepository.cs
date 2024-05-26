using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class OrderGuestDetailRepository :
        GenericRepository<OrderGuestDetailEntity>,
        IOrderGuestDetailRepository
    {
        public OrderGuestDetailRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
