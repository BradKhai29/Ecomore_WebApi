using Microsoft.Extensions.Configuration;
using Options.Models.Base;

namespace Options.Models
{
    public class ProtectionOptions : AppOptions
    {
        private const string ParentSectionName = "Others";
        private const string SectionName = "DataProtection";

        public string ApplicationDiscriminator { get; set; }
        public string PurposeKey { get; set; }

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(ParentSectionName)
                .GetRequiredSection(SectionName)
                .Bind(this);
        }
    }
}
