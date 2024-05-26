using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class SystemAccountService :
        ISystemAccountService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IPasswordService _passwordService;

        public SystemAccountService(
            IUnitOfWork<AppDbContext> unitOfWork,
            IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public Task<bool> CheckRegistrationConfirmationByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.SystemAccountRepository.IsFoundByExpressionAsync(
                findExpresison: systemAccount => systemAccount.Email.Equals(email)
                && systemAccount.AccountStatusId == AccountStatuses.EmailConfirmed.Id,
                cancellationToken: cancellationToken);
        }

        public Task<SystemAccountEntity> FindAccountByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.SystemAccountRepository.FindByEmailForLoginAsync(
                email: email,
                cancellationToken: cancellationToken);
        }

        public Task<bool> IsAccountAvailableByIdAsync(
            Guid accountId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.SystemAccountRepository.IsFoundByExpressionAsync(
                findExpresison: account 
                    => account.Id == accountId
                    && account.AccountStatusId == AccountStatuses.EmailConfirmed.Id,
                cancellationToken: cancellationToken);
        }

        public async Task<bool> UpdatePasswordByAccountIdAsync(
            Guid systemAccountId,
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

                    var passwordHash = _passwordService.GetHashPassword(newPassword);

                    await _unitOfWork.SystemAccountRepository.BulkUpdatePasswordByAccountIdAsync(
                        systemAccountId: systemAccountId,
                        passwordHash: passwordHash,
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
    }
}
