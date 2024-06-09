namespace WebApi.DTOs.Implementation.Orders.Outgoings
{
    public class OrderHistoryItemDto
    {
        public Guid Id { get; set; }
        /// <summary>
        ///     Gets and sets the order code for this order.
        /// </summary>
        /// <remarks>
        ///     This order code can be used for 
        ///     later payment cancel or payment approve.
        /// </remarks>
        public long OrderCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal TotalPrice { get; set; }

        public int ItemCount { get; set; }

        public int OrderStatus { get; set; }
    }
}
