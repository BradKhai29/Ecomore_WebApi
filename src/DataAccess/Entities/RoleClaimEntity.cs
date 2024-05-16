using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class RoleClaimEntity : IdentityRoleClaim<Guid>, IEntity
    {
        #region MetaData
        public static class MetaData
        {
            public const string TableName = "RoleClaims";
        }
        #endregion
    }
}
