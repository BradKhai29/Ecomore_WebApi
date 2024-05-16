using Microsoft.Extensions.Configuration;
using Options.Commons.Constants;

namespace Options.Models.Jwts.UserAccount
{
    public sealed class RegisterConfirmationTokenOptions : JwtRegisterConfirmationTokenOptions
    {
        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(AppSettingsSections.Area.UserAccount)
                .GetRequiredSection(AppSettingsSections.SubArea.Authentication)
                .Bind(SectionName);
        }
    }
}
