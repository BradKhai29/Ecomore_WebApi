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
    }
}
