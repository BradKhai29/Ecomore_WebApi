using WebApi.DTOs.Base;

namespace WebApi.DTOs.Implementation.Categories.Incomings
{
    public sealed class UpdateCategoryDto : IDtoNormalization
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public void NormalizeAllProperties()
        {
            Name = Name.Trim();
        }
    }
}
