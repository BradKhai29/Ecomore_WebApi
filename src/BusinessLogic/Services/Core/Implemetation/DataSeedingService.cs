using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.Commons.SystemConstants;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Options.Models;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class DataSeedingService : IDataSeedingService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly DefaultSystemAccountOptions _defaultSystemAccountOptions;
        private readonly IPasswordService _passwordService;

        public DataSeedingService(
            IUnitOfWork<AppDbContext> unitOfWork,
            DefaultSystemAccountOptions defaultSystemAccountOptions,
            IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _defaultSystemAccountOptions = defaultSystemAccountOptions;
            _passwordService = passwordService;
        }

        public async Task<bool> SeedAsync(CancellationToken cancellationToken)
        {
            bool notAllowSeeding = await _unitOfWork.SystemAccountRepository.IsFoundByExpressionAsync(
                findExpresison: account => account.Id == DefaultValues.SystemId,
                cancellationToken: cancellationToken);

            if (notAllowSeeding)
            {
                return false;
            }

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);
                
                    await AddAccountStatusesAsync(cancellationToken: cancellationToken);
                    await AddDefaultSystemAccountAsync(cancellationToken: cancellationToken);
                    await AddDoNotRemoveUserAsync(cancellationToken: cancellationToken);

                    await AddRolesAsync(cancellationToken: cancellationToken);
                    await AddCategoriesAsync(cancellationToken);
                    await AddProductStatusesAsync(cancellationToken);
                    await AddProductsAsync(cancellationToken);
                    await AddOrderStatusesAsync(cancellationToken);
                    await AddPaymentMethodsAsync(cancellationToken);

                    await _unitOfWork.SaveChangesToDatabaseAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }          
            });

            return true;
        }

        #region Private Methods
        private async Task AddAccountStatusesAsync(CancellationToken cancellationToken)
        {
            var statuses = AccountStatuses.GetValues();

            await _unitOfWork.AccountStatusRepository.AddRangeAsync(
                newEntities: statuses,
                cancellationToken: cancellationToken);
        }

        private async Task AddDefaultSystemAccountAsync(CancellationToken cancellationToken)
        {
            var account = new SystemAccountEntity
            {
                Id = DefaultValues.SystemId,
                Email = _defaultSystemAccountOptions.Email,
                PasswordHash = _passwordService.GetHashPassword(password: _defaultSystemAccountOptions.Password),
                AccountStatusId = AccountStatuses.EmailConfirmed.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.SystemAccountRepository.AddAsync(
                newEntity: account,
                cancellationToken: cancellationToken);
        }

        private Task<IdentityResult> AddDoNotRemoveUserAsync(CancellationToken cancellationToken)
        {
            var user = new UserEntity
            {
                Id = AppUsers.DoNotRemove.Id,
                FullName = UserEntity.GetFullName("none", "none"),
                UserName = AppUsers.DoNotRemove.UserName,
                PasswordHash = AppUsers.DoNotRemove.PasswordHash,
                AccountStatusId = AccountStatuses.EmailConfirmed.Id,
                Gender = true,
                Email = "abc@gmail.com",
                CreatedAt = DateTime.UtcNow,
                AvatarUrl = "unknown",
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true,
                PhoneNumber = "0123456789",
            };

            return _unitOfWork.UserRepository.AddAsync(user);
        }

        private async Task AddRolesAsync(CancellationToken cancellationToken)
        {
            var roles = new List<RoleEntity>
            {
                new() { Id = AppRoles.System.Id, Name = AppRoles.System.Name },
                new() { Id = AppRoles.Admin.Id, Name = AppRoles.Admin.Name },
                new() { Id = AppRoles.Employee.Id, Name = AppRoles.Employee.Name },
                new() { Id = AppRoles.DoNotRemove.Id, Name = AppRoles.DoNotRemove.Name }
            };

            foreach (var role in roles)
            {
                await _unitOfWork.RoleRepository.AddAsync(role);
            }
        }

        private async Task AddCategoriesAsync(CancellationToken cancellationToken)
        {
            var categories = new List<CategoryEntity>
            {
                new()
                {
                    Id = Categories.ProcessedNut.Id,
                    Name = Categories.ProcessedNut.Name,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = DefaultValues.SystemId
                }
            };

            await _unitOfWork.CategoryRepository.AddRangeAsync(categories, cancellationToken);
        }

        private async Task AddProductStatusesAsync(CancellationToken cancellationToken)
        {
            var statuses = ProductStatuses.GetValues();

            await _unitOfWork.ProductStatusRepository.AddRangeAsync(
                newEntities: statuses,
                cancellationToken: cancellationToken);
        }

        private async Task AddProductsAsync(CancellationToken cancellationToken)
        {
            var categoryId = Categories.ProcessedNut.Id;
            var products = new List<ProductEntity>();

            for (int i = 1; i <= 4; i++)
            {
                ProductEntity product =
                    new()
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = categoryId,
                        Name = $"Hat dinh duong [{i}]",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = DefaultValues.SystemId,
                        ProductStatusId = ProductStatuses.InStock.Id,
                        Description = "San pham ngon",
                        QuantityInStock = 100,
                        UnitPrice = 10_000,
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = DefaultValues.SystemId
                    };

                ProductImageEntity productImage =
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        FileName = $"hat_dinh_duong_{i}.jpg",
                        StorageUrl =
                            "https://nangmo.vn/wp-content/uploads/2021/04/mix-hat-dinh-duong-4.jpg"
                    };

                product.ProductImages = new List<ProductImageEntity>() { productImage };

                products.Add(product);
            }

            await _unitOfWork.ProductRepository.AddRangeAsync(products, cancellationToken);
        }

        private async Task AddOrderStatusesAsync(CancellationToken cancellationToken)
        {
            var orderStatuses = OrderStatuses.GetValues();

            await _unitOfWork.OrderStatusRepository.AddRangeAsync(orderStatuses, cancellationToken);
        }

        private async Task AddPaymentMethodsAsync(CancellationToken cancellationToken)
        {
            var paymentMethods = PaymentMethods.GetValues();

            await _unitOfWork.PaymentMethodRepository.AddRangeAsync(
                paymentMethods,
                cancellationToken);
        }
        #endregion
    }
}
