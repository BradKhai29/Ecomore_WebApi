using BusinessLogic.Services.Core.Base;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.Categories.Outgoings;
using WebApi.DTOs.Implementation.Products.Outgoings;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class UserProductController : ControllerBase
    {
        private readonly IUserProductService _productService;

        public UserProductController(IUserProductService userProductService)
        {
            _productService = userProductService;
        }

        [HttpGet("all/{pageSize:int}")]
        public async Task<IActionResult> GetAllProductsAsync(
            int pageSize,
            CancellationToken cancellationToken)
        {
            if (pageSize < 1)
            {
                pageSize = 100;
            }

            var products = await _productService.GetAllProductsAsync(
                pageSize: pageSize,
                cancellationToken: cancellationToken);

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

        [HttpGet("{productId}")]
        public async Task<IActionResult> FindProductByIdForDetailDisplayAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var foundProduct = await _productService.FindProductByIdForDetailDisplayAsync(
                productId: productId,
                cancellationToken: cancellationToken);

            if (Equals(foundProduct, null))
            {
                string errorMessage = $"Product with id [{productId}] is not found.";

                return NotFound(ApiResponse.Failed(errorMessage));
            }

            var productDto = new GetProductByIdForDetailDisplayDto
            {
                Id = productId,
                Category = new GetCategoryByIdForDetailDisplayDto
                {
                    Id = foundProduct.Category.Id,
                    Name = foundProduct.Category.Name
                },
                Name = foundProduct.Name,
                Description = foundProduct.Description,
                UnitPrice = foundProduct.UnitPrice,
                QuantityInStock = foundProduct.QuantityInStock,
                Status = foundProduct.ProductStatus.Name,
                ImageUrls = foundProduct.ProductImages.Select(image => image.StorageUrl)
            };

            return Ok(ApiResponse.Success(productDto));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> FindAllProductsByCategoryIdAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            var products = await _productService.FindAllProductsByCategoryIdAsync(
                categoryId: categoryId,
                cancellationToken: cancellationToken);

            if (Equals(products, null))
            {
                return NoContent();
            }

            var productDtos = products.Select(item => new GetProductForGeneralDisplayDto
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

            return Ok(ApiResponse.Success(productDtos));
        }
    }
}
