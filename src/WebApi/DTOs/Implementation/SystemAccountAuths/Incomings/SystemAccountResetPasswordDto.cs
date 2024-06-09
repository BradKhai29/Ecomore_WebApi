using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Implementation.SystemAccountAuths.Incomings
{
    public class SystemAccountResetPasswordDto
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
