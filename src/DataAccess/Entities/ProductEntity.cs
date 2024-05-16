using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public partial class ProductEntity : GuidEntity, ICreatedEntity, IUpdatedEntity
    {
        public Guid CategoryId { get; set; }

        public string Name { get; set; }

        public decimal UnitPrice { get; set; }

        public string Description { get; set; }

        public Guid ProductStatusId { get; set; }

        public int QuantityInStock { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UpdatedBy { get; set; }

        #region Relationships
        public CategoryEntity Category { get; set; }

        public ProductStatusEntity ProductStatus { get; set; }

        public IEnumerable<ProductImageEntity> ProductImages { get; set; }

        public IEnumerable<OrderItemEntity> OrderItems { get; set; }

        public SystemAccountEntity Creator { get; set; }

        public SystemAccountEntity Updater { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "Products";
        }
        #endregion
    }
}
