using BusinessLogic.Models;
using BusinessLogic.Services.Core.Base;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class UserService : IUserService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public UserService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<bool> CheckRegistrationConfirmationByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.UserRepository.IsFoundByExpressionAsync(expression: user
                => user.Email.Equals(email)
                && user.EmailConfirmed == true,
                cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindUserByIdForProfileDisplayAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.UserRepository.FindUserByIdForProfileDisplayAsync(
                userId: userId,
                cancellationToken: cancellationToken);
        }

        public Task<UserEntity> FindUserByEmailAsync(string email)
        {
            if (Equals(email, null))
            {
                return Task.FromResult(default(UserEntity));
            }

            var isEmail = email.Contains('@');

            return _unitOfWork.UserRepository.Manager.FindByEmailAsync(email);
        }

        public Task<bool> IsUserExistedByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.UserRepository.IsFoundByExpressionAsync(
                expression: user => user.Id == userId,
                cancellationToken: cancellationToken);
        }

        public Task<bool> CheckUserBannedStatusByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.UserRepository.IsFoundByExpressionAsync(
                expression: user => user.Email.Equals(email)
                && user.AccountStatusId == AccountStatuses.Banned.Id,
                cancellationToken: cancellationToken);
        }

        public async Task<bool> UpdateUserProfileAsync(
            UserEntity userProfile,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    await _unitOfWork.UserRepository.BulkUpdateUserProfileAsync(
                        userToUpdate: userProfile,
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

        public async Task<bool> UpdateUserPasswordAsync(
            UserEntity foundUser,
            string newPassword,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    foundUser.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.UserRepository.Manager.RemovePasswordAsync(foundUser);

                    await _unitOfWork.UserRepository.Manager.AddPasswordAsync(foundUser, newPassword);

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

        public async Task<bool> UpdateUserPasswordAsyncByUserId(
            Guid userId,
            string newPassword,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    var foundUser = await _unitOfWork.UserRepository.Manager.FindByIdAsync(userId: userId.ToString());

                    if (foundUser != null)
                    {
                        foundUser.UpdatedAt = DateTime.UtcNow;

                        await _unitOfWork.UserRepository.Manager.RemovePasswordAsync(foundUser);

                        await _unitOfWork.UserRepository.Manager.AddPasswordAsync(foundUser, newPassword);

                        await _unitOfWork.SaveChangesToDatabaseAsync(cancellationToken);

                        await _unitOfWork.CommitTransactionAsync(cancellationToken);

                        result = true;
                    }
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

        public async Task<bool> UpdateUserAvatarAsync(
            UserEntity userToUpdate,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken);

                    await _unitOfWork.UserRepository.BulkUpdateUserAvatarAsync(
                        userToUpdate: userToUpdate,
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

        public async Task<AppResult<UserEntity>> CheckPasswordByUserIdAsync(
            Guid userId,
            string password)
        {
            var foundUser = await _unitOfWork.UserRepository.FindByIdAsync(userId);

            if (Equals(foundUser, null))
            {
                return AppResult<UserEntity>.Failed($"User with Id [{userId}] is not found.");
            }

            var isSamePassword = await _unitOfWork.UserRepository.Manager.CheckPasswordAsync(foundUser, password);

            if (isSamePassword)
            {
                return AppResult<UserEntity>.Success(foundUser);
            }

            return AppResult<UserEntity>.Failed("Old password is not correct.");
        }

        public Task<UserEntity> FindUserByIdForVerificationAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.UserRepository.FindUserByIdForVerificationAsync(
                userId: userId,
                cancellationToken: cancellationToken);
        }
    }
}
