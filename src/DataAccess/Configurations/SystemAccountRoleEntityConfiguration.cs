using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class SystemAccountRoleEntityConfiguration
        : IEntityConfiguration<SystemAccountRoleEntity>
    {
        public void Configure(EntityTypeBuilder<SystemAccountRoleEntity> builder)
        {
            builder.ToTable(SystemAccountRoleEntity.MetaData.TableName);

            builder.HasKey(systemAccountRole => new
            {
                systemAccountRole.SystemAccountId,
                systemAccountRole.RoleId
            });
        }
    }
}
