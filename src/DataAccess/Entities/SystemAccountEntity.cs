using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class SystemAccountEntity : GuidEntity
    {
        public Guid AccountStatusId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

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
