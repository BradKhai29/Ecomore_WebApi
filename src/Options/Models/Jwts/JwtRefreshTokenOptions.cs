namespace Options.Models.Jwts
{
    public abstract class JwtRefreshTokenOptions : JwtOptions
    {
        public const string SectionName = "RefreshToken";

        public int LiveDays { get; set; }
    }
}
