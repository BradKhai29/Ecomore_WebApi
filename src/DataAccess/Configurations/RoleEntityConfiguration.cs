using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class RoleEntityConfiguration : IEntityConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.ToTable(RoleEntity.MetaData.TableName);

            // Properties Configuration.
            builder
                .Property(role => role.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_50)
                .IsRequired();

            builder.HasIndex(role => role.Name).IsUnique();

            #region Relationships
            builder
                .HasMany(role => role.SystemAccountRoles)
                .WithOne(systemAccountRole => systemAccountRole.Role);
            #endregion
        }
    }
}
