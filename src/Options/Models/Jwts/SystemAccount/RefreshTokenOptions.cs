using Microsoft.Extensions.Configuration;
using Options.Commons.Constants;

namespace Options.Models.Jwts.SystemAccount
{
    public sealed class RefreshTokenOptions :
        JwtRefreshTokenOptions
    {
        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(AppSettingsSections.Area.SystemAccount)
                .GetRequiredSection(AppSettingsSections.SubArea.Authentication)
                .GetRequiredSection(SectionName)
                .Bind(this);
        }
    }
}
