using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class CategoryEntity : GuidEntity, ICreatedEntity
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        #region Relationships
        public SystemAccountEntity Creator { get; set; }

        public IEnumerable<ProductEntity> Products { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "Categories";
        }
        #endregion
    }
}
