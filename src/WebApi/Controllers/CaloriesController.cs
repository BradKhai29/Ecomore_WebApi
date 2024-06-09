using BusinessLogic.Services.Core.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.CaloriesCalulation.Incomings;
using WebApi.DTOs.Implementation.CaloriesCalulation.Outgoings;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaloriesController : ControllerBase
    {
        private readonly IUserProductService _userProductService;

        public CaloriesController(IUserProductService userProductService)
        {
            _userProductService = userProductService;
        }

        [HttpPost]
        public async Task<IActionResult> CalculateAndRecommendProductAsync(
            CustomerPhysicalInfoDto physicalInfoDto,
            CancellationToken cancellationToken)
        {
            var standardCaloriesNeed = physicalInfoDto.CalculateStandardCaloriesNeed();
            var recommendCaloriesNeed = (int)
                Math.Ceiling(ExerciseIntensityHelper.GetIntensityCoefficient(physicalInfoDto.Intensity) 
                * standardCaloriesNeed);

            var randomProducts = await _userProductService.GetRandomProductsAsync(3, cancellationToken);

            return Ok(ApiResponse.Success(new CaloriesResultAndRecommendationDto
            {
                StandardCalories = standardCaloriesNeed,
                RecommendedCalories = recommendCaloriesNeed,
                Products = randomProducts.Select(item => new DTOs.Implementation.Products.Outgoings.GetProductForGeneralDisplayDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category.Name,
                    ImageUrls = item.ProductImages.Select(image => image.StorageUrl),
                    UnitPrice = item.UnitPrice,
                })
            }));
        }
    }
}
