using BusinessLogic.Services.Core.Base;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class UserAuthService : IUserAuthService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public UserAuthService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ConfirmUserRegistrationAsync(
            UserEntity user,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    user = await _unitOfWork.UserRepository.FindByIdAsync(id: user.Id);

                    user.AccountStatusId = AccountStatuses.EmailConfirmed.Id;
                    user.EmailConfirmed = true;

                    await _unitOfWork.UserRepository.UpdateAsync(foundEntity: user);

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

        public Task<bool> IsEmailRegisteredAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.UserRepository.IsFoundByExpressionAsync(
                expression: user => user.Email.Equals(email),
                cancellationToken: cancellationToken);
        }

        public async Task<bool> RegisterUserAsync(
            UserEntity newUser,
            string password,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.UserRepository.Manager.CreateAsync(user: newUser, password: password);

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

        public async Task<IdentityResult> ValidatePasswordAsync(
            UserEntity user,
            string password)
        {
            var manager = _unitOfWork.UserRepository.Manager;
            var passwordValidators = _unitOfWork.UserRepository.Manager.PasswordValidators;

            if (Equals(passwordValidators, null))
            {
                return IdentityResult.Success;
            }

            foreach (var validator in passwordValidators)
            {
                var validationResult = await validator.ValidateAsync(manager: manager, user: user, password: password);

                if (!validationResult.Succeeded)
                {
                    return validationResult;
                }
            }

            return IdentityResult.Success;
        }
    }
}
