namespace WebApi.DTOs.Implementation.ShoppingCarts.Outgoings
{
    public class OutputShoppingCartDto
    {
        public Guid CartId { get; set; }

        public IEnumerable<OutputCartItemDto> CartItems { get; set; }

        public decimal TotalPrice { get; set; }

        public static OutputShoppingCartDto Empty(Guid cartId)
        {
            return new OutputShoppingCartDto
            {
                CartId = cartId,
                CartItems = new List<OutputCartItemDto>(),
                TotalPrice = 0
            };
        }
    }
}
