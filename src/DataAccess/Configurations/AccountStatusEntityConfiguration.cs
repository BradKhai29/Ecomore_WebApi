using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class AccountStatusEntityConfiguration : IEntityConfiguration<AccountStatusEntity>
    {
        public void Configure(EntityTypeBuilder<AccountStatusEntity> builder)
        {
            builder.ToTable(AccountStatusEntity.MetaData.TableName);

            builder.HasKey(status => status.Id);

            builder
                .Property(status => status.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_20)
                .IsRequired();

            builder.HasIndex(status => status.Name).IsUnique();

            #region Relationships
            builder
                .HasMany(accountStatus => accountStatus.Users)
                .WithOne(user => user.AccountStatus);

            builder
                .HasMany(accountStatus => accountStatus.SystemAccounts)
                .WithOne(account => account.AccountStatus);
            #endregion
        }
    }
}
