using BusinessLogic.Models;
using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;
using Options.Models;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class SystemAccountAuthService :
        ISystemAccountAuthService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly SystemAccountLoginConstraintsOptions _loginConstraintsOptions;
        private readonly IPasswordService _passwordService;

        public SystemAccountAuthService(
            IUnitOfWork<AppDbContext> unitOfWork,
            SystemAccountLoginConstraintsOptions loginConstraintsOptions,
            IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _loginConstraintsOptions = loginConstraintsOptions;
            _passwordService = passwordService;
        }

        public Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken)
        {
            return _unitOfWork.SystemAccountRepository.IsFoundByExpressionAsync(
                findExpresison: systemAccount => systemAccount.Email.Equals(email),
                cancellationToken: cancellationToken);
        }

        public async Task<AppResult<SystemAccountEntity>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            var foundAccount = await _unitOfWork.SystemAccountRepository.FindByEmailForLoginAsync(
                email: email,
                cancellationToken: cancellationToken);

            var isLockoutEnd = 
                foundAccount.AccessFailedCount == _loginConstraintsOptions.MaxAccessFailedCount
                && foundAccount.LockoutEnd < DateTime.Now;

            if (!isLockoutEnd)
            {
                return AppResult<SystemAccountEntity>.Failed(
                    $"You have been locked out to [{foundAccount.LockoutEnd}] because failed to login after {_loginConstraintsOptions.MaxAccessFailedCount} times.");
            }

            var passwordHash = _passwordService.GetHashPassword(password);

            var isValidPassword = foundAccount.PasswordHash.Equals(passwordHash);

            if (isValidPassword)
            {
                return AppResult<SystemAccountEntity>.Success(foundAccount);
            }

            var result = AppResult<SystemAccountEntity>.Failed("Invalid login credentials.");
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                    var isMaxAccessFailedCount =
                        foundAccount.AccessFailedCount == _loginConstraintsOptions.MaxAccessFailedCount;

                    if (isMaxAccessFailedCount)
                    {
                        if (isLockoutEnd)
                        {
                            // Reset the access failed count to 1 due to user
                            // has retry to login again after the lockout time.
                            foundAccount.AccessFailedCount = 1;

                            await _unitOfWork.SystemAccountRepository.BulkUpdateSystemAccountForFailedAccessAsync(
                                systemAccountEntity: foundAccount,
                                cancellationToken: cancellationToken);

                            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                            result.ErrorMessages = new List<string>(1)
                            {
                                "Invalid login credentials"
                            };
                        }
                        else
                        {
                            result.ErrorMessages = new List<string>(1)
                            {
                                $"You have been lockout to [{foundAccount.LockoutEnd}] because failed to login after {_loginConstraintsOptions.MaxAccessFailedCount} times."
                            };
                        }
                    }
                    else
                    {
                        foundAccount.AccessFailedCount++;

                        // If the access-failed-count reach the limit.
                        if (foundAccount.AccessFailedCount == _loginConstraintsOptions.MaxAccessFailedCount)
                        {
                            foundAccount.LockoutEnd = DateTime.Now.AddMinutes(
                                value: _loginConstraintsOptions.LockoutMinutes);

                            result.ErrorMessages = new List<string>(1)
                            {
                                $"You have been locked out to [{foundAccount.LockoutEnd}] because failed to login after {_loginConstraintsOptions.MaxAccessFailedCount} times."
                            };
                        }

                        await _unitOfWork.SystemAccountRepository.BulkUpdateSystemAccountForFailedAccessAsync(
                            systemAccountEntity: foundAccount,
                            cancellationToken: cancellationToken);

                        await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
                    }
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
    }
}
