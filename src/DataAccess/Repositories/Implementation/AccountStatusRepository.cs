using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class AccountStatusRepository :
        GenericRepository<AccountStatusEntity>,
        IAccountStatusRepository
    {
        public AccountStatusRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
