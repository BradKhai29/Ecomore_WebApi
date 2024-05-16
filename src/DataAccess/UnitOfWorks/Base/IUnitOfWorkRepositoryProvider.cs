using DataAccess.Repositories.Base;

namespace DataAccess.UnitOfWorks.Base
{
    /// <summary>
    ///     The base interface to implement UnitOfWork with repository.
    /// </summary>
    public interface IUnitOfWorkRepositoryProvider
    {
        ICategoryRepository CategoryRepository { get; }

        IProductRepository ProductRepository { get; }

        ISystemAccountRepository SystemAccountRepository { get; }

        IAccountStatusRepository AccountStatusRepository { get; }

        IUserRepository UserRepository { get; }

        IRoleRepository RoleRepository { get; }

        IProductStatusRepository ProductStatusRepository { get; }

        IOrderStatusRepository OrderStatusRepository { get; }

        IPaymentMethodRepository PaymentMethodRepository { get; }

        IUserTokenRepository UserTokenRepository { get; }
    }
}
