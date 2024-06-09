using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class RoleRepository :
        IdentityRepository<RoleEntity>,
        IRoleRepository
    {
        // Backing fields.
        private readonly RoleManager<RoleEntity> _roleManager;

        public RoleRepository(
            DbContext dbContext,
            RoleManager<RoleEntity> roleManager) : base(dbContext)
        {
            _roleManager = roleManager;
        }

        public RoleManager<RoleEntity> Manager => _roleManager;

        public override Task<IdentityResult> AddAsync(RoleEntity newEntity)
        {
            return _roleManager.CreateAsync(role: newEntity);
        }

        public override Task<RoleEntity> FindByIdAsync(Guid id)
        {
            return _roleManager.FindByIdAsync(roleId: id.ToString());
        }

        public override Task<RoleEntity> FindByNameAsync(string name)
        {
            return _roleManager.FindByNameAsync(roleName: name);
        }

        public override async Task<IdentityResult> RemoveAsync(Guid id)
        {
            var foundUser = await _roleManager.FindByIdAsync(roleId: id.ToString());

            return await _roleManager.DeleteAsync(foundUser);
        }

        public override Task<IdentityResult> UpdateAsync(RoleEntity foundEntity)
        {
            return _roleManager.UpdateAsync(role: foundEntity);
        }
    }
}
