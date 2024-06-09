using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class UserTokenEntityConfiguration : IEntityConfiguration<UserTokenEntity>
    {
        public void Configure(EntityTypeBuilder<UserTokenEntity> builder)
        {
            builder.ToTable(UserTokenEntity.MetaData.TableName);

            builder
                .Property(token => token.Id)
                .IsRequired();

            builder
                .HasIndex(token => token.Id)
                .IsClustered(false)
                .IsUnique(true);

            builder
                .Property(token => token.Value)
                .HasColumnType(SqlDataTypes.SqlServer.VARCHAR_50)
                .IsRequired();

            builder
                .Property(userToken => userToken.ExpiredAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();
        }
    }
}
