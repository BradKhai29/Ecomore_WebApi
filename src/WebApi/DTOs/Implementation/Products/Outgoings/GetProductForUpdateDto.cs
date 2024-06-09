using WebApi.DTOs.Implementation.Categories.Outgoings;
using WebApi.DTOs.Implementation.ProductImages.Outgoings;
using WebApi.DTOs.Implementation.ProductStatuses.Outgoings;

namespace WebApi.DTOs.Implementation.Products.Outgoings
{
    public sealed class GetProductForUpdateDto
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }

        public GetCategoryByIdForDetailDisplayDto Category { get; set; }

        public ProductStatusDetailDto Status { get; set; }

        public string Description { get; set; }

        public int QuantityInStock { get; set; }

        public decimal UnitPrice { get; set; }

        public IEnumerable<ProductImageDetailDto> ProductImages { get; set; }
    }
}
