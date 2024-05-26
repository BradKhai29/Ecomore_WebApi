using BusinessLogic.Services.Core.Base;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class CategoryService :
        ICategoryService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public CategoryService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddCategoryAsync(
            CategoryEntity category,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    await _unitOfWork.CategoryRepository.AddAsync(category, cancellationToken);

                    await _unitOfWork.SaveChangesToDatabaseAsync(cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken);                    
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken);
                }
            });

            return result;
        }

        public async Task<bool> DeleteCategoryByIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            var hasProducts = await _unitOfWork.ProductRepository.IsFoundByExpressionAsync(
                findExpresison: product => product.CategoryId == categoryId,
                cancellationToken: cancellationToken);

            if (hasProducts)
            {
                return false;
            }

            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    await _unitOfWork.CategoryRepository.BulkDeleteByIdAsync(categoryId, cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken);                    
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken);
                }
            });

            return result;
        }

        public Task<IEnumerable<CategoryEntity>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            return _unitOfWork.CategoryRepository.GetAllAsync(cancellationToken: cancellationToken);
        }

        public Task<bool> CheckCategoryHasProductByIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.ProductRepository.IsFoundByExpressionAsync(
                findExpresison: product => product.CategoryId == categoryId,
                cancellationToken: cancellationToken);
        }

        public async Task<bool> UpdateCategoryAsync(
            CategoryEntity category,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    await _unitOfWork.CategoryRepository.BulkUpdateAsync(
                        categoryToUpdate: category,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken);                    
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken);
                }
            });

            return result;
        }

        public Task<bool> IsCategoryExistedByNameAsync(string categoryName, CancellationToken cancellationToken)
        {
            return _unitOfWork.CategoryRepository.IsFoundByExpressionAsync(
                findExpresison: category => category.Name.Equals(categoryName),
                cancellationToken: cancellationToken);
        }

        public Task<bool> IsCategoryExistedByIdAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            return _unitOfWork.CategoryRepository.IsFoundByExpressionAsync(
                findExpresison: category => category.Id == categoryId,
                cancellationToken: cancellationToken);
        }
    }
}
