using Microsoft.IdentityModel.Tokens;
using Options.Models.Base;
using System.Text;

namespace Options.Models.Jwts
{
    public abstract class JwtOptions : AppOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public SymmetricSecurityKey GetSecurityKey()
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);

            return new SymmetricSecurityKey(key: key);
        }
    }
}
