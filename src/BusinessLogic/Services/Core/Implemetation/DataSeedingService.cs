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

            var result = false;
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
                    result = true;
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

            return result;
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
            var categories = Categories.GetValues();

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
            var products = new List<ProductEntity>(8);
            
            var product1 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.Nut.Id,
                Name = "Hạt mắc ca",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 180_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product1.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product1.Id,
                    FileName = $"{product1.Id}.jpg",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942803/Ecomore/Products/macca_rgf3ms.jpg",
                }
            };

            var product2 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.Nut.Id,
                Name = "Hạt hạnh nhân",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 160_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product2.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product2.Id,
                    FileName = $"hat_{product2.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942791/Ecomore/Products/almond_ga5kb5.jpg",
                }
            };

            var product3 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.ProcessedNut.Id,
                Name = "Hạt óc chó sấy",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 160_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product3.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product3.Id,
                    FileName = $"hat_{product3.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942793/Ecomore/Products/wallnut_f8j3qw.jpg",
                }
            };

            var product4 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.ProcessedNut.Id,
                Name = "Hạt điều rang muối",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 200_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product4.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product4.Id,
                    FileName = $"hat_{product4.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942793/Ecomore/Products/cashew_oqw6ur.jpg",
                }
            };

            var product5 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.NutMilk.Id,
                Name = "Sữa hạt hạnh nhân",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 25_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product5.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product5.Id,
                    FileName = $"hat_{product5.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942802/Ecomore/Products/almond_milk_nbahg7.jpg",
                }
            };

            var product6 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.NutMilk.Id,
                Name = "Sữa hạt mắc ca",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 25_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product6.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product6.Id,
                    FileName = $"hat_{product6.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942791/Ecomore/Products/macadamia_milk_lkwns6.jpg",
                }
            };

            var product7 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.NutMilk.Id,
                Name = "Sữa hạt óc chó",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 25_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product7.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product7.Id,
                    FileName = $"hat_{product7.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942794/Ecomore/Products/walnut_milk_o2ehur.jpg",
                }
            };

            var product8 = new ProductEntity
            {
                Id = Guid.NewGuid(),
                CategoryId = Categories.NutMilk.Id,
                Name = "Sữa hạt điều",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DefaultValues.SystemId,
                ProductStatusId = ProductStatuses.InStock.Id,
                Description = "Sản phẩm ngon",
                QuantityInStock = 100,
                UnitPrice = 25_000,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };
            product8.ProductImages = new List<ProductImageEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product8.Id,
                    FileName = $"hat_{product1.Id}",
                    StorageUrl = "https://res.cloudinary.com/dsjsmbdpw/image/upload/v1717942791/Ecomore/Products/cashew_milk_bkvoa1.jpg",
                }
            };

            products.Add(product1);
            products.Add(product2);
            products.Add(product3);
            products.Add(product4);
            products.Add(product5);
            products.Add(product6);
            products.Add(product7);
            products.Add(product8);

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
