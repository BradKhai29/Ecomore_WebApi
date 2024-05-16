using System.Text;
using DataAccess.Entities.Base;

namespace DataAccess.Entities
{
    public class OrderEntity : GuidEntity, IUpdatedEntity
    {
        public Guid StatusId { get; set; }

        public Guid UserId { get; set; }

        public Guid GuestId { get; set; }

        public Guid PaymentMethodId { get; set; }

        /// <summary>
        ///     Used to store the order code that received from
        ///     the <b>3rd-party online banking provider</b>
        ///     or the <b>system auto-generated</b>.
        /// </summary>
        public long OrderCode { get; set; }

        public string OrderNote { get; set; }

        public decimal TotalPrice { get; set; }

        public string DeliveredAddress { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime DeliveredAt { get; set; }

        public static long GenerateOrderCode(DateTime dateTime)
        {
            const int length = 16;

            // Format date and time for extraction (modify if needed)
            string formattedDateTime = dateTime.ToString("yyyyMMddHHmmss");
            var codeBuilder = new StringBuilder(formattedDateTime);
            int codeLength = length - formattedDateTime.Length;

            codeBuilder.Append(GenerateRandomNumberCode(codeLength));
            return long.Parse(codeBuilder.ToString());
        }

        public static string GenerateRandomNumberCode(int length)
        {
            // Use a secure random number generator
            Random random = new();

            // Build the code string with random digits
            StringBuilder code = new(length);
            for (int i = 0; i < length; i++)
            {
                code.Append(random.Next(0, 10)); // Generate digit between 0 and 9 (inclusive)
            }

            return code.ToString();
        }

        #region Relationships
        public UserEntity User { get; set; }

        public OrderGuestDetailEntity OrderGuestDetail { get; set; }

        public PaymentMethodEntity PaymentMethod { get; set; }

        public SystemAccountEntity Updater { get; set; }

        public IEnumerable<OrderItemEntity> OrderItems { get; set; }

        public OrderStatusEntity Status { get; set; }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string TableName = "Orders";
        }
        #endregion
    }
}
