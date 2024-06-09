using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class SystemAccountTokenEntityConfiguration
        : IEntityConfiguration<SystemAccountTokenEntity>
    {
        public void Configure(EntityTypeBuilder<SystemAccountTokenEntity> builder)
        {
            builder.ToTable(SystemAccountTokenEntity.MetaData.TableName);

            builder.HasKey(token => token.Id);

            builder
                .Property(token => token.Name)
                .IsRequired();

            builder
                .Property(token => token.Value)
                .HasColumnType(SqlDataTypes.SqlServer.VARCHAR_50)
                .IsRequired();

            builder
                .Property(token => token.SystemAccountId)
                .IsRequired();

            builder
                .Property(token => token.CreatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder
                .Property(token => token.ExpiredAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            #region Relationships
            builder
                .HasOne(token => token.SystemAccount)
                .WithMany(systemAccount => systemAccount.SystemAccountTokens)
                .HasPrincipalKey(token => token.Id)
                .HasForeignKey(token => token.SystemAccountId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
