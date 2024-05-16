using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    /// <summary>
    ///     This table is only used when
    ///     the order is purchased by guest (not logged-in user).
    /// </summary>
    public class OrderGuestDetailEntity : IEntity
    {
        public Guid OrderId { get; set; }

        public string GuestName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        #region Relationships
        public OrderEntity Order { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "OrderGuestDetails";
        }
        #endregion
    }
}
