using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface IUserService
    {
        Task<bool> IsUserExistedByIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task<bool> CheckRegistrationConfirmationByEmailAsync(
            string email,
            CancellationToken cancellationToken);

        Task<bool> CheckUserBannedStatusByEmailAsync(
            string email,
            CancellationToken cancellationToken);

        Task<UserEntity> FindUserByIdForProfileDisplayAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task<UserEntity> FindUserByEmailAsync(string email);

        Task<bool> UpdateUserProfileAsync(
            UserEntity userProfile,
            CancellationToken cancellationToken);

        Task<bool> UpdateUserPasswordAsyncByUserId(
            Guid userId,
            string newPassword,
            CancellationToken cancellationToken);
    }
}
