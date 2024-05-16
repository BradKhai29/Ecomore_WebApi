using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class OrderItemEntity : IEntity
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public decimal SellingPrice { get; set; }

        public int SellingQuantity { get; set; }

        #region Relationships
        public OrderEntity Order { get; set; }

        public ProductEntity Product { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "OrderItems";
        }
        #endregion
    }
}
