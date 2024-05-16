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
        public static IServiceCollection AddAppOptions<TAppOptions>(
            this IServiceCollection services,
            ConfigurationManager configurationManager
        )
            where TAppOptions : AppOptions, new()
        {
            TAppOptions appOptions = new();
            appOptions.Bind(configuration: configurationManager);

            services.AddSingleton(implementationInstance: appOptions);

            return services;
        }

        public static IServiceCollection AddOptionsConfiguration(
            this IServiceCollection services,
            ConfigurationManager configurationManager
        )
        {
            #region SystemAccount JwtTokenOptions.
            services.AddAppOptions<Options.Models.Jwts.SystemAccount.AccessTokenOptions>(configurationManager);
            services.AddAppOptions<Options.Models.PasswordHashOptions>(configurationManager);
            #endregion

            #region UserAccount JwtTokenOptions.
            services.AddAppOptions<Options.Models.Jwts.UserAccount.AccessTokenOptions>(configurationManager);
            services.AddAppOptions<Options.Models.Jwts.UserAccount.RefreshTokenOptions>(configurationManager);
            services.AddAppOptions<Options.Models.Jwts.UserAccount.RegisterConfirmationTokenOptions>(configurationManager);
            services.AddAppOptions<Options.Models.Jwts.UserAccount.ResetPasswordTokenOptions>(configurationManager);
            #endregion

            services.AddAppOptions<MailOptions>(configurationManager);
            services.AddAppOptions<PayOsOptions>(configurationManager);
            services.AddAppOptions<DefaultSystemAccountOptions>(configurationManager);

            return services;
        }
    }
}
