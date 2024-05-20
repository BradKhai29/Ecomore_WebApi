using BusinessLogic.Models;
using DataAccess.Entities;

namespace BusinessLogic.Services.Core.Base
{
    public interface ISystemAccountAuthService
    {
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
        ///     Try to find the system account with the input <paramref name="email"/>
        ///     and <paramref name="password"/> to process the login operation.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The <see cref="AppResult"/> instance that represents the result of login operation.
        /// </returns>
        Task<AppResult<SystemAccountEntity>> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);
    }
}
