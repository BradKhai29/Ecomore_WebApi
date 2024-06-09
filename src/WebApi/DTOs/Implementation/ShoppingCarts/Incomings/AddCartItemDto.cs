using System.ComponentModel.DataAnnotations;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.ShoppingCarts.Incomings
{
    public sealed class AddCartItemDto
    {
        [IsValidGuid]
        public Guid CartId { get; set; }

        [IsValidGuid]
        public Guid ProductId { get; set; }

        [Required]
        [Range(minimum: 1, maximum: 100)]
        public int Quantity { get; set; }
    }
}
