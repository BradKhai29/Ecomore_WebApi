using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ISystemAccountService
    {
        /// <summary>
        ///     Check if the system account with specified 
        ///     input <paramref name="accountId"/>
        ///     is still available or not.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsAccountAvailableByIdAsync(
            Guid accountId,
            CancellationToken cancellationToken);

        Task<bool> CheckRegistrationConfirmationByEmailAsync(
            string email,
            CancellationToken cancellationToken);

        Task<SystemAccountEntity> FindAccountByEmailAsync(
            string email,
            CancellationToken cancellationToken);

        Task<bool> UpdatePasswordByAccountIdAsync(
            Guid systemAccountId,
            string newPassword,
            CancellationToken cancellationToken);
    }
}
