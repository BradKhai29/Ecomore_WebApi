using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class RoleEntity : IdentityRole<Guid>, IGuidEntity
    {
        #region Relationships
        public IEnumerable<SystemAccountRoleEntity> SystemAccountRoles { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "Roles";
        }
        #endregion
    }
}
