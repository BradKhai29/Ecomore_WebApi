using System.ComponentModel.DataAnnotations;
using WebApi.DTOs.Base;

namespace WebApi.DTOs.Implementation.SystemAccountAuths.Incomings
{
    public sealed class SystemAccountLoginDto : IDtoNormalization
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public void NormalizeAllProperties()
        {
            Email = Email.Trim().ToLower();
        }
    }
}
