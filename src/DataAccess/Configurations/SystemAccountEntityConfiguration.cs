using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class SystemAccountEntityConfiguration : IEntityConfiguration<SystemAccountEntity>
    {
        public void Configure(EntityTypeBuilder<SystemAccountEntity> builder)
        {
            builder.ToTable(SystemAccountEntity.MetaData.TableName);

            builder.HasKey(account => account.Id);

            builder
                .Property(account => account.Email)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200)
                .IsRequired();

            builder
                .HasIndex(account => account.Email)
                .IsClustered(false)
                .IsUnique();

            builder
                .Property(account => account.PasswordHash)
                .IsRequired();

            builder
                .Property(account => account.CreatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder
                .Property(account => account.UpdatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            #region Relationships
            builder
                .HasMany(systemAccount => systemAccount.SystemAccountRoles)
                .WithOne(accountRole => accountRole.SystemAccount)
                .HasPrincipalKey(systemAccount => systemAccount.Id)
                .HasForeignKey(accountRole => accountRole.SystemAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(systemAccount => systemAccount.CreatedProducts)
                .WithOne(product => product.Creator)
                .HasPrincipalKey(systemAccount => systemAccount.Id)
                .HasForeignKey(product => product.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(systemAccount => systemAccount.UpdatedProducts)
                .WithOne(product => product.Updater)
                .HasPrincipalKey(systemAccount => systemAccount.Id)
                .HasForeignKey(product => product.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(systemAccount => systemAccount.ManagedOrders)
                .WithOne(order => order.Updater)
                .HasPrincipalKey(systemAccount => systemAccount.Id)
                .HasForeignKey(order => order.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
