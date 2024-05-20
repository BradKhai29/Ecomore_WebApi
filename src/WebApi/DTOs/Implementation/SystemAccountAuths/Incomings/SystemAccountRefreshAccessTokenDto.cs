using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs.Implementation.SystemAccountAuths.Incomings
{
    /// <summary>
    ///     This dto contains both access-token and refresh-token
    ///     that need for application doing refresh these tokens.
    /// </summary>
    public class SystemAccountRefreshAccessTokenDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
