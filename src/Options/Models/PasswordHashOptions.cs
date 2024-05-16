using Microsoft.Extensions.Configuration;
using Options.Commons.Constants;
using Options.Models.Base;

namespace Options.Models
{
    public class PasswordHashOptions : AppOptions
    {
        public const string SectionName = "PasswordHash";

        public string SecretKey { get; set; }

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(AppSettingsSections.Area.SystemAccount)
                .GetRequiredSection(AppSettingsSections.SubArea.Authentication)
                .GetRequiredSection(SectionName)
                .Bind(instance: this);
        }
    }
}
