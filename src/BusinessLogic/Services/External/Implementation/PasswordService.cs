using BusinessLogic.Services.External.Base;
using Microsoft.IdentityModel.Tokens;
using Options.Models;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services.External.Implementation
{
    internal class PasswordService :
        IPasswordService
    {
        private readonly PasswordHashOptions _passwordHashOptions;
        private readonly HMACSHA256 _hasher;

        public PasswordService(PasswordHashOptions passwordHashOptions)
        {
            _passwordHashOptions = passwordHashOptions;
            _hasher = new HMACSHA256(key: Encoding.UTF8.GetBytes(_passwordHashOptions.SecretKey));
        }

        public string GetHashPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            var hashPasswordBytes = _hasher.ComputeHash(passwordBytes);

            return Base64UrlEncoder.Encode(hashPasswordBytes);
        }
    }
}
