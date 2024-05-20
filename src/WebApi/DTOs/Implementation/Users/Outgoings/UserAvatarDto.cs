namespace WebApi.DTOs.Implementation.Users.Outgoings
{
    public sealed class UserAvatarDto
    {
        public Guid UserId { get; set; }

        public string AvatarUrl { get; set; }
    }
}
