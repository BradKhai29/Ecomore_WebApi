using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class RoleClaimEntityConfiguration : IEntityConfiguration<RoleClaimEntity>
    {
        public void Configure(EntityTypeBuilder<RoleClaimEntity> builder)
        {
            builder.ToTable(RoleClaimEntity.MetaData.TableName);
        }
    }
}
