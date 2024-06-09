using BusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using WebApi.DTOs.Implementation.ProductImages.Incomings;
using WebApi.Shared.Enums;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.Products.Incomings
{
    public sealed class UpdateProductDto
    {
        [IsValidGuid]
        public Guid Id { get; set; }

        [Required]
        [MinLength(length: 4)]
        public string ProductName { get; set; }

        [IsValidGuid]
        public Guid CategoryId { get; set; }

        [IsValidGuid]
        public Guid StatusId { get; set; }

        [Range(minimum: 10_000, maximum: 10_000_000)]
        public decimal UnitPrice { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(minimum: 1, maximum: 10_000)]
        public int QuantityInStock { get; set; }

        /// <summary>
        ///     Contain an encoded string that represent the
        ///     information of the list of upload image files.
        /// </summary>
        [Required]
        [MinLength(2)]
        public string MetaData { get; set; }

        [AllowFileRange(minRange: 1, maxRange: 4)]
        [AllowFileExtension(new string[] { "png", "jpg", "jpeg" })]
        public IFormFile[] ProductImageFiles { get; set; }

        public AppResult<IEnumerable<UpdateProductImageDto>> DecodeMetaData()
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var decodeItems = JsonSerializer.Deserialize<IEnumerable<UpdateProductImageDto>>(
                json: MetaData,
                options: jsonSerializerOptions);

            if (Equals(decodeItems, null))
            {
                return AppResult<IEnumerable<UpdateProductImageDto>>.Failed();
            }

            return AppResult<IEnumerable<UpdateProductImageDto>>.Success(decodeItems);
        }
    }
}
