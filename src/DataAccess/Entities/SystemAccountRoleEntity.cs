using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class SystemAccountRoleEntity : IEntity
    {
        public Guid SystemAccountId { get; set; }

        public Guid RoleId { get; set; }

        #region Relationships
        public SystemAccountEntity SystemAccount { get; set; }

        public RoleEntity Role { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "SystemAccountRoles";
        }
        #endregion
    }
}
