namespace Options.Models.Jwts
{
    public abstract class JwtAccessTokenOptions : JwtOptions
    {
        public const string SectionName = "AccessToken";

        public int LiveMinutes { get; set; }
    }
}
