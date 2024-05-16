namespace Options.Models.Jwts
{
    public abstract class JwtRegisterConfirmationTokenOptions : JwtOptions
    {
        public const string SectionName = "RegisterConfirmationToken";

        public int LiveMinutes { get; set; }
    }
}
