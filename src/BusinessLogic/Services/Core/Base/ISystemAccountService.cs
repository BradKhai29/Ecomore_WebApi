using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ISystemAccountService
    {
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
