using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.Core.Implemetation;
using BusinessLogic.Services.External.Base;
using BusinessLogic.Services.External.Implementation;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic
{
    public static class DependencyInjection
    {
        /// <summary>
        ///     Inject the services from the BusinessLogic layer.
        /// </summary>
        /// <param name="services">Current Service Collection</param>
        /// <returns>
        ///     The current service collection after injecting BusinessLogic layer's services.
        /// </returns>
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddDataAccess();

            services.AddCoreServices();
            services.AddExternalServices();

            return services;
        }

        /// <summary>
        ///     Add the services that interact
        ///     with core-entites to the dependency container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // User services section.
            services.AddScoped<IDataSeedingService, DataSeedingService>();
            services.AddScoped<IUserProductService, UserProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserTokenService, UserTokenService>();
            services.AddScoped<IUserAuthService, UserAuthService>();

            // System-Account (Admin) services section.

            return services;
        }

        private static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IMailService, GmailService>();

            return services;
        }
    }
}
