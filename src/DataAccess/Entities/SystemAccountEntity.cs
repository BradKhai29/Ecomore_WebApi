using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class SystemAccountEntity : GuidEntity
    {
        public Guid AccountStatusId { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public int AccessFailedCount { get; set; }

        /// <summary>
        ///     Gets or sets the date and time, in UTC, when this system account lockout ends.
        /// </summary>
        /// <remarks>
        ///     A value in the past means the user is not locked out.
        /// </remarks>
        public DateTime LockoutEnd { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        #region Relationships
        public AccountStatusEntity AccountStatus { get; set; }

        public IEnumerable<SystemAccountTokenEntity> SystemAccountTokens { get; set; }

        public IEnumerable<SystemAccountRoleEntity> SystemAccountRoles { get; set; }

        public IEnumerable<OrderEntity> ManagedOrders { get; set; }

        public IEnumerable<CategoryEntity> CreatedCategories { get; set; }

        public IEnumerable<ProductEntity> CreatedProducts { get; set; }

        public IEnumerable<ProductEntity> UpdatedProducts { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "SystemAccounts";
        }
        #endregion
    }
}
