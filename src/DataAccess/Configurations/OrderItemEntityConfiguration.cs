using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class OrderItemEntityConfiguration : IEntityConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            builder.ToTable(OrderItemEntity.MetaData.TableName);

            builder.HasKey(orderItem => new { orderItem.OrderId, orderItem.ProductId, });

            builder
                .Property(orderItem => orderItem.SellingPrice)
                .HasColumnType(SqlDataTypes.SqlServer.DECIMAL_12_02)
                .IsRequired();

            builder.Property(orderItem => orderItem.SellingQuantity).IsRequired();
        }
    }
}
