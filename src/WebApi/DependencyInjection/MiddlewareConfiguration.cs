using Presentation.Middlewares;

namespace WebApi.DependencyInjection
{
    public static class MiddlewareConfiguration
    {
        public static IServiceCollection AddMiddlewareConfiguration(this IServiceCollection services)
        {
            services.AddScoped<GuestIdCookieMiddleware>();

            return services;
        }
    }
}
