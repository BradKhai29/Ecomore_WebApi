using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Shared.AppConstants;
using WebApi.Shared.AuthenticationHandlers;

namespace WebApi.DependencyInjection
{
    public static class AuthenticationConfiguration
    {
        public static IServiceCollection AddAuthenticationConfiguration(
            this IServiceCollection services
        )
        {
            // More details: https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddScoped<SecurityTokenHandler, JwtSecurityTokenHandler>();

            // Add authentication-scheme handlers.
            services.AddScoped<UserAccountSchemeAuthenticationHandler>();
            services.AddScoped<SystemAccountSchemeAuthenticationHandler>();

            services.AddAuthentication(options =>
            {
                
                // Add the custom authentication schemes and its authentication handler.
                options.AddScheme<UserAccountSchemeAuthenticationHandler>(
                    name: CustomAuthenticationSchemes.UserAccountScheme,
                    displayName: CustomAuthenticationSchemes.UserAccountScheme);

                options.AddScheme<SystemAccountSchemeAuthenticationHandler>(
                    name: CustomAuthenticationSchemes.SystemAccountScheme,
                    displayName: CustomAuthenticationSchemes.SystemAccountScheme);

                options.DefaultScheme = CustomAuthenticationSchemes.UserAccountScheme;
            });

            return services;
        }
    }
}
