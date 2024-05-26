using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Implementation;
using DataAccess.UnitOfWorks.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccess.UnitOfWorks.Implementations;

public class AppUnitOfWork<TContext> : IUnitOfWork<TContext>
    where TContext : DbContext
{
    // Backing stores.
    private readonly TContext _dbContext;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;

    // Backing fields for transaction handling.
    private IDbContextTransaction _dbTransaction;

    // Backing fields for implement UnitOfWork Repository Provider.
    private IProductRepository _productRepository;
    private IProductImageRepository _productImageRepository;
    private IProductStatusRepository _productStatusRepository;
    private ISystemAccountRepository _systemAccountRepository;
    private IAccountStatusRepository _accountStatusRepository;
    private IUserRepository _userRepository;
    private IRoleRepository _roleRepository;
    private ICategoryRepository _categoryRepository;
    private IOrderStatusRepository _orderStatusRepository;
    private IPaymentMethodRepository _paymentMethodRepository;
    private IUserTokenRepository _userTokenRepository;
    private ISystemAccountTokenRepository _systemAccountTokenRepository;
    private IHealthCheckRepository _healthCheckRepository;
    private IOrderRepository _orderRepository;
    private IOrderItemRepository _orderItemRepository;
    private IOrderGuestDetailRepository _orderGuestDetailRepository;

    // Properties.
    public IProductRepository ProductRepository
    {
        get
        {
            _productRepository ??= new ProductRepository(_dbContext);

            return _productRepository;
        }
    }

    public IProductImageRepository ProductImageRepository
    {
        get
        {
            _productImageRepository ??= new ProductImageRepository(_dbContext);

            return _productImageRepository;
        }
    }

    public ISystemAccountRepository SystemAccountRepository
    {
        get
        {
            _systemAccountRepository ??= new SystemAccountRepository(_dbContext);

            return _systemAccountRepository;
        }
    }

    public IAccountStatusRepository AccountStatusRepository
    {
        get
        {
            _accountStatusRepository ??= new AccountStatusRepository(_dbContext);

            return _accountStatusRepository;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            _userRepository ??= new UserRepository(_dbContext, _userManager);

            return _userRepository;
        }
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            _roleRepository ??= new RoleRepository(_dbContext, _roleManager);

            return _roleRepository;
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            _categoryRepository ??= new CategoryRepository(_dbContext);

            return _categoryRepository;
        }
    }

    public IProductStatusRepository ProductStatusRepository
    {
        get
        {
            _productStatusRepository ??= new ProductStatusRepository(_dbContext);

            return _productStatusRepository;
        }
    }

    public IOrderRepository OrderRepository
    {
        get
        {
            _orderRepository ??= new OrderRepository(_dbContext);

            return _orderRepository;
        }
    }

    public IOrderItemRepository OrderItemRepository
    {
        get
        {
            _orderItemRepository ??= new OrderItemRepository(_dbContext);

            return _orderItemRepository;
        }
    }

    public IOrderGuestDetailRepository OrderGuestDetailRepository
    {
        get
        {
            _orderGuestDetailRepository ??= new OrderGuestDetailRepository(_dbContext);

            return _orderGuestDetailRepository;
        }
    }

    public IOrderStatusRepository OrderStatusRepository
    {
        get
        {
            _orderStatusRepository ??= new OrderStatusRepository(_dbContext);

            return _orderStatusRepository;
        }
    }

    public IPaymentMethodRepository PaymentMethodRepository
    {
        get
        {
            _paymentMethodRepository ??= new PaymentMethodRepository(_dbContext);

            return _paymentMethodRepository;
        }
    }

    public IUserTokenRepository UserTokenRepository
    {
        get
        {
            _userTokenRepository ??= new UserTokenRepository(_dbContext);

            return _userTokenRepository;
        }
    }

    public ISystemAccountTokenRepository SystemAccountTokenRepository
    {
        get
        {
            _systemAccountTokenRepository ??= new SystemAccountTokenRepository(_dbContext);

            return _systemAccountTokenRepository;
        }
    }

    public IHealthCheckRepository HealthCheckRepository
    {
        get
        {
            _healthCheckRepository ??= new HealthCheckRepository(_dbContext);

            return _healthCheckRepository;
        }
    }

    public AppUnitOfWork(
        TContext dbContext,
        UserManager<UserEntity> userManager,
        RoleManager<RoleEntity> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _dbContext.Database.CreateExecutionStrategy();
    }

    public async Task CreateTransactionAsync(CancellationToken cancellationToken)
    {
        _dbTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        return _dbTransaction.CommitAsync(cancellationToken);
    }

    public ValueTask DisposeTransactionAsync(CancellationToken cancellationToken)
    {
        return _dbTransaction.DisposeAsync();
    }

    public Task RollBackTransactionAsync(CancellationToken cancellationToken)
    {
        return _dbTransaction.RollbackAsync(cancellationToken);
    }

    public Task SaveChangesToDatabaseAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
