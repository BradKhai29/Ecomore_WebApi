using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class UserLoginEntity : IdentityUserLogin<Guid>, IEntity
    {
        #region MetaData
        public static class MetaData
        {
            public const string TableName = "UserLogins";
        }
        #endregion
    }
}
