namespace WebApi.DTOs.Implementation.Orders.Outgoings
{
    public class OrderItemDto
    {
        /// <summary>
        ///     The productId of this order item.
        /// </summary>
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        ///     The selling quantity of this order item.
        /// </summary>
        public int SellingQuantity { get; set; }

        /// <summary>
        ///     The selling price of this order item
        ///     at the time the order is created.
        /// </summary>
        /// <remarks>
        ///     The selling price of this item may
        ///     be different to the unit price of
        ///     the product if the product has any change
        ///     in price during the time customer purchase their order.
        /// </remarks>
        public decimal SellingPrice { get; set; }

        public decimal SubTotal => SellingPrice * SellingQuantity;
    }
}
