﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Options.Commons.Constants;
using Options.Models.Base;
using System.Text;

namespace Options.Models
{
    public class PayOsOptions : AppOptions
    {
        public const string SectionName = "PayOS";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int LiveMinutes { get; set; }

        public string ClientId { get; set; }

        public string ApiKey { get; set; }

        public string ChecksumKey { get; set; }

        public string PrivateKey { get; set; }

        public string ReturnUrl { get; set; }

        public string CancelUrl { get; set; }

        public SymmetricSecurityKey GetSecurityKey()
        {
            var key = Encoding.UTF8.GetBytes(PrivateKey);

            return new SymmetricSecurityKey(key: key);
        }

        public TimeSpan GetLifeSpan() => TimeSpan.FromMinutes(LiveMinutes);

        public override void Bind(IConfiguration configuration)
        {
            configuration
                .GetRequiredSection(AppSettingsSections.Area.Payment)
                .GetRequiredSection(SectionName)
                .Bind(instance: this);
        }
    }
}
