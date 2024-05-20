using System.ComponentModel.DataAnnotations;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.Users.Incomings
{
    public sealed class UpdateUserAvatarDto
    {
        [IsValidGuid]
        public Guid UserId { get; set; }

        [Required]
        [AllowFileExtension(Extensions = new string[] {"png", "jpg", "jpeg"})]
        public IFormFile AvatarImageFile { get; set; }
    }
}
