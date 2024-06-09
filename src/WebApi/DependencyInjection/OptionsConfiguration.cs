using Microsoft.Extensions.Options;
using Options.Models;
using Options.Models.Base;

namespace WebApi.DependencyInjection
{
    /// <summary>
    ///     This class contains extension methods for all <see cref="IOptions{T}"/>
    ///     configuration in this application.
    /// </summary>
    public static class OptionsConfiguration
    {
        private static IServiceCollection AddAppOptions<TAppOptions>(
            this IServiceCollection services,
            IConfiguration configuration
        )
            where TAppOptions : AppOptions, new()
        {
            TAppOptions appOptions = new();
            appOptions.Bind(configuration: configuration);

            services.AddSingleton(implementationInstance: appOptions);

            return services;
        }

        public static IServiceCollection AddOptionsConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            #region SystemAccount JwtTokenOptions.
            services.AddAppOptions<Options.Models.Jwts.SystemAccount.AccessTokenOptions>(configuration);
            services.AddAppOptions<Options.Models.Jwts.SystemAccount.RefreshTokenOptions>(configuration);
            services.AddAppOptions<Options.Models.Jwts.SystemAccount.ResetPasswordTokenOptions>(configuration);
            services.AddAppOptions<Options.Models.Jwts.SystemAccount.RegisterConfirmationTokenOptions>(configuration);
            services.AddAppOptions<SystemAccountLoginConstraintsOptions>(configuration);
            services.AddAppOptions<PasswordHashOptions>(configuration);
            #endregion

            #region UserAccount JwtTokenOptions.
            services.AddAppOptions<Options.Models.Jwts.UserAccount.AccessTokenOptions>(configuration);
            services.AddAppOptions<Options.Models.Jwts.UserAccount.RefreshTokenOptions>(configuration);
            services.AddAppOptions<Options.Models.Jwts.UserAccount.RegisterConfirmationTokenOptions>(configuration);
            services.AddAppOptions<Options.Models.Jwts.UserAccount.ResetPasswordTokenOptions>(configuration);
            #endregion

            #region Others
            services.AddAppOptions<MailOptions>(configuration);
            services.AddAppOptions<PayOsOptions>(configuration);
            services.AddAppOptions<DefaultSystemAccountOptions>(configuration);
            services.AddAppOptions<CloudinaryOptions>(configuration);
            services.AddAppOptions<ProtectionOptions>(configuration);
            #endregion

            return services;
        }
    }
}
