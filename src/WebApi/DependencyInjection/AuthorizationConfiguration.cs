namespace WebApi.DependencyInjection
{
    public static class AuthorizationConfiguration
    {
        public static IServiceCollection AddAuthorizationConfiguration(
            this IServiceCollection services
        )
        {
            services.AddAuthorization();

            return services;
        }
    }
}
