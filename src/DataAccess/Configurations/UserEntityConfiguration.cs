using DataAccess.Commons.SqlConstants;
using DataAccess.Commons.SystemConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class UserEntityConfiguration : IEntityConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable(UserEntity.MetaData.TableName);

            // Properties Configuration.
            builder
                .Property(user => user.FullName)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200)
                .IsRequired();

            builder
                .Property(user => user.AvatarUrl)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200)
                .HasDefaultValue(DefaultValues.UserAvatarUrl)
                .IsRequired();

            builder.Property(user => user.Email).HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200);

            builder.Property(user => user.AccountStatusId).IsRequired();

            builder
                .Property(user => user.CreatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder
                .Property(user => user.UpdatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            #region Relationships
            builder
                .HasMany(user => user.Orders)
                .WithOne(order => order.User)
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
