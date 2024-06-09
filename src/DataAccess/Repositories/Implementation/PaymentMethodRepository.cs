using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class PaymentMethodRepository :
        GenericRepository<PaymentMethodEntity>,
        IPaymentMethodRepository
    {
        public PaymentMethodRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
