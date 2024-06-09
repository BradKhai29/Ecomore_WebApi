using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class AccountStatusEntity : GuidEntity
    {
        public string Name { get; set; }

        #region Relationships
        public IEnumerable<UserEntity> Users { get; set; }

        public IEnumerable<SystemAccountEntity> SystemAccounts { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "AccountStatuses";
        }
        #endregion
    }
}
