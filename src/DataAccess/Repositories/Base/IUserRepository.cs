using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Repositories.Base
{
    public interface IUserRepository :
        IIdentityRepository<UserEntity, Guid>
    {
        UserManager<UserEntity> Manager { get; }

        Task<UserEntity> FindUserByIdForProfileDisplayAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task<UserEntity> FindUserByIdForVerificationAsync(
            Guid userId,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Bulk update the specified user's profile
        ///     from the input user new credentials.
        /// </summary>
        /// <param name="userToUpdate">
        ///     The new credentials of the target user
        ///     that need to be updated.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> BulkUpdateUserProfileAsync(
            UserEntity userToUpdate,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Bulk update the specified user's avatar.
        /// </summary>
        Task<int> BulkUpdateUserAvatarAsync(
            UserEntity userToUpdate,
            CancellationToken cancellationToken);
    }
}
