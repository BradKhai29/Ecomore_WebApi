using Microsoft.Extensions.Configuration;
using Options.Commons.Constants;
using Options.Models.Base;

namespace Options.Models
{
    public sealed class CloudinaryOptions : AppOptions
    {
        public const string SectionName = "Cloudinary";
        public string CloudinaryUrl { get; set; }

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(AppSettingsSections.Area.DistributedService)
                .GetRequiredSection(SectionName)
                .Bind(this);
        }
    }
}
