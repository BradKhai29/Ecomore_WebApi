using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class UserRoleEntity : IdentityUserRole<Guid>, IEntity
    {
        #region MetaData
        public static class MetaData
        {
            public const string TableName = "UserRoles";
        }
        #endregion
    }
}
