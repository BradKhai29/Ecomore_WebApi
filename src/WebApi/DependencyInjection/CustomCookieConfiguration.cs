using Presentation.Middlewares;

namespace WebApi.DependencyInjection
{
    public static class CustomCookieConfiguration
    {
        public static IServiceCollection AddCustomCookieConfiguration(
            this IServiceCollection services
        )
        {
            services.AddScoped<GuestIdCookieMiddleware>();

            return services;
        }
    }
}
