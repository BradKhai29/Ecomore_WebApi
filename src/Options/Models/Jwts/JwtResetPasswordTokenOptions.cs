namespace Options.Models.Jwts
{
    public abstract class JwtResetPasswordTokenOptions : JwtOptions
    {
        public const string SectionName = "ResetPasswordToken";

        public int LiveMinutes { get; set; }
    }
}
