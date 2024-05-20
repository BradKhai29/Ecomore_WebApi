using BusinessLogic.Models;
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

        Task<AppResult<UserEntity>> CheckPasswordByUserIdAsync(
            Guid userId,
            string password);

        Task<UserEntity> FindUserByIdForProfileDisplayAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task<UserEntity> FindUserByEmailAsync(string email);

        /// <summary>
        ///     Update the user profile basic information
        ///     such as: Full name, gender and phone number (if have).
        /// </summary>
        /// <param name="userProfile">
        ///     The user profile's credentials that need to update.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task<bool> UpdateUserProfileAsync(
            UserEntity userProfile,
            CancellationToken cancellationToken);

        Task<bool> UpdateUserAvatarAsync(
            UserEntity userToUpdate,
            CancellationToken cancellationToken);

        Task<bool> UpdateUserPasswordAsyncByUserId(
            Guid userId,
            string newPassword,
            CancellationToken cancellationToken);

        Task<bool> UpdateUserPasswordAsync(
            UserEntity user,
            string newPassword,
            CancellationToken cancellationToken);
    }
}
