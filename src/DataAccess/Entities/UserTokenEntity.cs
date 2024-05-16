using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    public class UserTokenEntity : IdentityUserToken<Guid>, IGuidEntity
    {
        public Guid Id { get; set; }

        [NotMapped]
        public DateTime CreatedAt { get; set; }

        public DateTime ExpiredAt { get; set; }

        #region Relationships
        [NotMapped]
        public UserEntity User { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "UserTokens";
        }
        #endregion
    }
}
