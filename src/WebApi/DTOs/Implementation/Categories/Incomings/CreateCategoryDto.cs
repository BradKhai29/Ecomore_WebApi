using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;

namespace WebApi.DTOs.Implementation.Categories.Incomings
{
    public sealed class CreateCategoryDto : IDtoNormalization
    {
        [Required]
        public string Name { get; set; }

        public void NormalizeAllProperties()
        {
            Name = Name.Trim();
        }
    }
}
