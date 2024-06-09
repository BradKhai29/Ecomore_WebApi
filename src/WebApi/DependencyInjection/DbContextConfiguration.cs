using DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace WebApi.DependencyInjection
{
    public static class DbContextConfiguration
    {
        private const string LocalSectionName = "LocalDB";
        private const string RemoteSectionName = "RemoteDB";

        public static IServiceCollection AddDbContextConfiguration(
            this IServiceCollection services,
            ConfigurationManager configurationManager
        )
        {
            services.AddDbContextPool<AppDbContext>(optionsAction: options =>
            {
                var connectionString = configurationManager.GetConnectionString(RemoteSectionName);

                options.UseNpgsql(
                    connectionString: connectionString,
                    npgsqlOptionsAction: providerOptions =>
                    {
                        // Reference: https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency#custom-execution-strategy
                        providerOptions.EnableRetryOnFailure(maxRetryCount: 3);
                    });
            });

            return services;
        }

        private static ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder
                    .AddConsole()
                    .AddFilter(
                        category: DbLoggerCategory.Database.Command.Name,
                        level: LogLevel.Information
                    );
            });

            return services.BuildServiceProvider().GetService<ILoggerFactory>();
        }
    }
}
