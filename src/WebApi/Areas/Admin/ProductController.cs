using BusinessLogic.Services.Core.Base;
using WebApi.DTOs.Implementation.Products.Outgoings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Areas.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IAdminProductService _productService;

        public ProductController(IAdminProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllProductsAsync(cancellationToken);

            var productDtos = products.Select(selector: item => new GetProductForGeneralDisplayDto
            {
                Id = item.Id,
                Category = item.Category.Name,
                Name = item.Name,
                Description = item.Description,
                QuantityInStock = item.QuantityInStock,
                Status = item.ProductStatus.Name,
                UnitPrice = item.UnitPrice,
                ImageUrls = item.ProductImages.Select(image => image.StorageUrl)
            });

            return Ok(value: ApiResponse.Success(body: productDtos));
        }
    }
}
