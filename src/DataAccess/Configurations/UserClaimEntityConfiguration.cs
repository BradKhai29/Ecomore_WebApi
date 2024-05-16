using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class UserClaimEntityConfiguration : IEntityConfiguration<UserClaimEntity>
    {
        public void Configure(EntityTypeBuilder<UserClaimEntity> builder)
        {
            builder.ToTable(UserClaimEntity.MetaData.TableName);
        }
    }
}
