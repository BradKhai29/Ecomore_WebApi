using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class OrderGuestDetailEntityConfiguration
        : IEntityConfiguration<OrderGuestDetailEntity>
    {
        public void Configure(EntityTypeBuilder<OrderGuestDetailEntity> builder)
        {
            builder.ToTable(OrderGuestDetailEntity.MetaData.TableName);

            builder.HasKey(detail => detail.OrderId);

            builder
                .Property(detail => detail.GuestName)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200)
                .IsRequired();

            builder
                .Property(detail => detail.Email)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_50)
                .IsRequired();

            builder
                .Property(detail => detail.PhoneNumber)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_20)
                .IsRequired();
        }
    }
}
