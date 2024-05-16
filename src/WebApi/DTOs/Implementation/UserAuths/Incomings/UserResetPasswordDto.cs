using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;

namespace WebApi.DTOs.Implementation.UserAuths.Incomings
{
    public class UserResetPasswordDto
    {
        [Required]
        public string ResetPasswordToken { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(otherProperty: nameof(Password))]
        public string AgainPassword { get; set; }
    }
}
