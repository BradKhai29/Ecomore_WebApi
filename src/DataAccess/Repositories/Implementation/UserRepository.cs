using DataAccess.Entities;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.Base.Generics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Implementation
{
    internal class UserRepository :
        IdentityRepository<UserEntity>,
        IUserRepository
    {
        // Backing fields.
        private readonly UserManager<UserEntity> _userManager;

        public UserRepository(
            DbContext dbContext,
            UserManager<UserEntity> userManager) : base(dbContext)
        {
            _userManager = userManager;
        }

        public UserManager<UserEntity> Manager => _userManager;

        public override Task<IdentityResult> AddAsync(UserEntity newEntity)
        {
            return _userManager.CreateAsync(user: newEntity);
        }

        public Task<int> BulkUpdateUserAvatarAsync(
            UserEntity userToUpdate,
            CancellationToken cancellationToken)
        {
            var updateDateTime = DateTime.UtcNow;

            return _dbSet
                .Where(user => user.Id == userToUpdate.Id)
                .ExecuteUpdateAsync(
                    setPropertyCalls: user => user
                        .SetProperty(
                            user => user.AvatarUrl,
                            user => userToUpdate.AvatarUrl)
                        .SetProperty(
                            user => user.UpdatedAt,
                            user => updateDateTime),
                    cancellationToken: cancellationToken);
        }

        public Task<int> BulkUpdateUserProfileAsync(
            UserEntity userToUpdate,
            CancellationToken cancellationToken)
        {
            var updateDateTime = DateTime.UtcNow;

            return _dbSet
                .Where(user => user.Id == userToUpdate.Id)
                .ExecuteUpdateAsync(
                    setPropertyCalls: user => user
                        .SetProperty(
                            user => user.FullName,
                            user => userToUpdate.FullName)
                        .SetProperty(
                            user => user.PhoneNumber,
                            user => userToUpdate.PhoneNumber)
                        .SetProperty(
                            user => user.Gender,
                            user => userToUpdate.Gender)
                        .SetProperty(
                            user => user.UpdatedAt,
                            user => updateDateTime),
                    cancellationToken: cancellationToken);
        }

        public override Task<UserEntity> FindByIdAsync(Guid id)
        {
            return _userManager.FindByIdAsync(userId: id.ToString());
        }

        public override Task<UserEntity> FindByNameAsync(string name)
        {
            return _userManager.FindByNameAsync(userName: name);
        }

        public Task<UserEntity> FindUserByIdForProfileDisplayAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _dbSet
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => new UserEntity
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public override async Task<IdentityResult> RemoveAsync(Guid id)
        {
            var foundUser = await _userManager.FindByIdAsync(userId: id.ToString());

            return await _userManager.DeleteAsync(foundUser);
        }

        public override Task<IdentityResult> UpdateAsync(UserEntity foundEntity)
        {
            return _userManager.UpdateAsync(user: foundEntity);
        }
    }
}
