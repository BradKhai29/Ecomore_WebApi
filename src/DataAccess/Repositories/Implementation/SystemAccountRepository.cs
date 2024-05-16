using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class SystemAccountRepository :
        GenericRepository<SystemAccountEntity>,
        ISystemAccountRepository
    {
        public SystemAccountRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
