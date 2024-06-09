using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class ProductImageEntity : GuidEntity
    {
        public Guid ProductId { get; set; }

        public int UploadOrder { get; set; }

        public string FileName { get; set; }

        public string StorageUrl { get; set; }

        #region Relationships
        public ProductEntity Product { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "ProductImages";
        }
        #endregion
    }
}
