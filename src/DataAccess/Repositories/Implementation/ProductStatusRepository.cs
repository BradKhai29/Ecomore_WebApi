using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class ProductStatusRepository :
        GenericRepository<ProductStatusEntity>,
        IProductStatusRepository
    {
        public ProductStatusRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
