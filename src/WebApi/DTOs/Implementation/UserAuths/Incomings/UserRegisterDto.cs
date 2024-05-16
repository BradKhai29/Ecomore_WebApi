using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;

namespace WebApi.DTOs.Implementation.UserAuths.Incomings
{
    public class UserRegisterDto : IDtoNormalization
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public void NormalizeAllProperties()
        {
            FirstName = FirstName.Trim();
            LastName = LastName.Trim();
            Email = Email.Trim().ToLower();
        }
    }
}
