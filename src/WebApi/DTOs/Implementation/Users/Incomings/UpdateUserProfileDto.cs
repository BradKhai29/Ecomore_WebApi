using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Implementation.Users.Incomings
{
    public class UpdateUserProfileDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public IFormFile AvatarImageFile { get; set; }
    }
}
