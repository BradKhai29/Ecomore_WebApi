using DataAccess.Entities;
using Helpers.ExtensionMethods;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataAccess.DbContexts
{
    public class AppDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplyEntityConfiguration(modelBuilder);
        }

        private void ApplyEntityConfiguration(ModelBuilder modelBuilder)
        {
            // IdentityServer default configuration.
            base.OnModelCreating(modelBuilder);

            RemoveAspNetPrefixInIdentityTable(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        private void RemoveAspNetPrefixInIdentityTable(ModelBuilder builder)
        {
            const string AspNetPrefix = "AspNet";
            int index = AspNetPrefix.Length;

            builder
                .Model.GetEntityTypes()
                .ForEach(action: entityType =>
                {
                    var tableName = entityType.GetTableName();

                    if (tableName.StartsWith(value: AspNetPrefix))
                    {
                        entityType.SetTableName(name: $"{tableName[index..]}");
                    }
                });
        }
    }
}
