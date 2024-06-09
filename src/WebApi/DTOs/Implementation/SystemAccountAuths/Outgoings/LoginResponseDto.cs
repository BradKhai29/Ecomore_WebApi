namespace WebApi.DTOs.Implementation.SystemAccountAuths.Outgoings
{
    public sealed class LoginResponseDto
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
