using WebApi.DTOs.Implementation.Products.Outgoings;

namespace WebApi.DTOs.Implementation.CaloriesCalulation.Outgoings
{
    public class CaloriesResultAndRecommendationDto
    {
        public int StandardCalories { get; set; }

        public int RecommendedCalories { get; set; }

        public IEnumerable<GetProductForGeneralDisplayDto> Products { get; set; }
    }
}
