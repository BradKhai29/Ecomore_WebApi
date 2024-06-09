using Microsoft.Extensions.Configuration;
using Options.Models.Base;

namespace Options.Models
{
    public class MailOptions : AppOptions
    {
        public const string LinkPlaceHolder = "{link}";
        public const string WebUrlHolder = "{webUrl}";
        public const string LogoUrlHolder = "{logoUrl}";

        public const string SectionName = "Mail";

        public string Address { get; set; }

        public string DisplayName { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string WebUrl { get; set; }

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(SectionName)
                .Bind(this);
        }
    }
}
