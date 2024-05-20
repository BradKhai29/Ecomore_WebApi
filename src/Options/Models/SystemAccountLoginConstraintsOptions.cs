using Microsoft.Extensions.Configuration;
using Options.Commons.Constants;
using Options.Models.Base;

namespace Options.Models
{
    public sealed class SystemAccountLoginConstraintsOptions : AppOptions
    {
        private const string SectionName = "LoginConstraints";

        public int MaxAccessFailedCount { get; set; }

        public int LockoutMinutes { get; set; }

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(AppSettingsSections.Area.SystemAccount)
                .GetRequiredSection(SectionName)
                .Bind(this);
        }
    }
}
