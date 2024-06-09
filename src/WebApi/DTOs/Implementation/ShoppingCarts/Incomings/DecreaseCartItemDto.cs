using System.ComponentModel.DataAnnotations;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.ShoppingCarts.Incomings
{
    public class DecreaseCartItemDto
    {
        [IsValidGuid]
        public Guid CartId { get; set; }

        [IsValidGuid]
        public Guid ProductId { get; set; }

        [Range(minimum: 1, maximum: 100)]
        public int Quantity { get; set; }
    }
}
