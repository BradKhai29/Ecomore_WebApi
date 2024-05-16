using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class OrderEntityConfiguration : IEntityConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable(OrderEntity.MetaData.TableName);

            builder.HasKey(order => order.Id);

            builder.Property(order => order.StatusId).IsRequired();

            builder.Property(order => order.UserId).IsRequired();

            builder.Property(order => order.GuestId).HasDefaultValue(Guid.Empty).IsRequired();

            builder.Property(order => order.PaymentMethodId).IsRequired();

            builder.Property(order => order.OrderCode).IsRequired();

            builder.HasIndex(order => order.OrderCode).IsClustered(false).IsUnique();

            builder
                .Property(order => order.OrderNote)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_500)
                .IsRequired(false);

            builder
                .Property(product => product.TotalPrice)
                .HasColumnType(SqlDataTypes.SqlServer.DECIMAL_12_02)
                .IsRequired();

            builder
                .Property(order => order.DeliveredAddress)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200)
                .IsRequired();

            builder
                .Property(order => order.CreatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder
                .Property(order => order.UpdatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder.Property(order => order.UpdatedBy).IsRequired();

            builder
                .Property(order => order.DeliveredAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            #region Relationships
            builder
                .HasMany(order => order.OrderItems)
                .WithOne(orderItem => orderItem.Order)
                .HasPrincipalKey(order => order.Id)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(order => order.OrderGuestDetail)
                .WithOne(orderGuestDetail => orderGuestDetail.Order)
                .HasPrincipalKey<OrderEntity>(order => order.Id)
                .HasForeignKey<OrderGuestDetailEntity>(detail => detail.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
