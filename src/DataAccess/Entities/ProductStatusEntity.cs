using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class ProductStatusEntity : GuidEntity
    {
        public string Name { get; set; }

        #region Relationships
        public IEnumerable<ProductEntity> Products { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "ProductStatuses";
        }
        #endregion
    }
}
