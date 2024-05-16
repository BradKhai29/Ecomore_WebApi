using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class TokenTypeEntity : GuidEntity
    {
        public string Name { get; set; }

        #region Relationships
        public IEnumerable<UserTokenEntity> UserTokens { get; set; }

        public IEnumerable<SystemAccountTokenEntity> SystemAccountTokens { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "TokenTypes";
        }
        #endregion
    }
}
