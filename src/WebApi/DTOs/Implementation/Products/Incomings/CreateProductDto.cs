using System.ComponentModel.DataAnnotations;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.Products.Incomings
{
    public sealed class CreateProductDto
    {
        [Required]
        [MinLength(length: 4)]
        public string ProductName { get; set; }

        [IsValidGuid]
        public Guid CategoryId { get; set; }

        [Range(minimum: 10_000, maximum: 10_000_000)]
        public decimal UnitPrice { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(minimum: 1, maximum: 10_000)]
        public int QuantityInStock { get; set; }

        [AllowFileRange(minRange: 1, maxRange: 4)]
        [AllowFileExtension(new string[] {"png", "jpg", "jpeg"})]
        public IFormFile[] ProductImageFiles { get; set; }
    }
}
