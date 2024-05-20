using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services.Core.Base
{
    public interface IUserAuthService
    {
        /// <summary>
        ///     Validate if the input password has valid format or not.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> ValidatePasswordAsync(
            UserEntity user,
            string password);

        /// <summary>
        ///     Check if the input email has been registered
        ///     in this system or not.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsEmailRegisteredAsync(
            string email,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Register a new user account based on the input register information.
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> RegisterUserAsync(
            UserEntity newUser,
            string password,
            CancellationToken cancellationToken);

        Task<bool> ConfirmUserRegistrationAsync(
            UserEntity user,
            CancellationToken cancellationToken);
    }
}
