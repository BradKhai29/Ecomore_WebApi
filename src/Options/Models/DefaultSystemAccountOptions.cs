using Microsoft.Extensions.Configuration;
using Options.Models.Base;

namespace Options.Models
{
    public class DefaultSystemAccountOptions : AppOptions
    {
        private const string ParentSectionName = "Others";
        private const string SectionName = "DefaultSystemAccount";

        public string Email { get; set; }

        public string Password { get; set; }

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(key: ParentSectionName)
                .GetRequiredSection(key: SectionName)
                .Bind(instance: this);
        }
    }
}
