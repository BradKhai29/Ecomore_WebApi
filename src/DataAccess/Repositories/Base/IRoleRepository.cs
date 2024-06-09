using DataAccess.Entities;
using DataAccess.Repositories.Base.Generics;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Repositories.Base
{
    public interface IRoleRepository :
        IIdentityRepository<RoleEntity, Guid>
    {
        RoleManager<RoleEntity> Manager { get; }
    }
}
