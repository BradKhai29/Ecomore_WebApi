using System.ComponentModel.DataAnnotations;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.Users.Incomings
{
    public sealed class UpdateUserPasswordDto
    {
        [IsValidGuid]
        public Guid UserId { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string AgainPassword { get; set; }
    }
}
