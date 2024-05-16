using DataAccess.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class SystemAccountTokenEntity : GuidEntity
    {
        public Guid SystemAccountId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the token.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the token value.
        /// </summary>
        public string Value { get; set; }

        public DateTime ExpiredAt { get; set; }

        #region Relationships
        public SystemAccountEntity SystemAccount { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "SystemAccountTokens";
        }
        #endregion
    }
}
