using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.DTOs.Implementation.Users.Incomings
{
    public class UpdateUserProfileDto : IDtoNormalization
    {
        [Required]
        [IsValidGuid]
        public Guid UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool Gender { get; set; }

        [IsValidPhoneNumber(NotAllowEmpty = false)]
        public string PhoneNumber { get; set; }

        public void NormalizeAllProperties()
        {
            FirstName = FirstName.Trim();
            LastName = LastName.Trim();
            PhoneNumber = PhoneNumber ?? "";
        }
    }
}
