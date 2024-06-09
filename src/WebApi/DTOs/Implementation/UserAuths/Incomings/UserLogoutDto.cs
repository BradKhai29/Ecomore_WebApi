using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Implementation.UserAuths.Incomings
{
    public class UserLogoutDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
