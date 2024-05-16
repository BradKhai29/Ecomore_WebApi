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
                var connectionString = configurationManager.GetConnectionString(LocalSectionName);

                options.UseSqlServer(connectionString);
                options.UseLoggerFactory(GetLoggerFactory());
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
