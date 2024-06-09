using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class OrderStatusEntityConfiguration : IEntityConfiguration<OrderStatusEntity>
    {
        public void Configure(EntityTypeBuilder<OrderStatusEntity> builder)
        {
            builder.ToTable(OrderStatusEntity.MetaData.TableName);

            builder.HasKey(status => status.Id);

            builder
                .Property(status => status.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_20)
                .IsRequired();

            builder.HasIndex(status => status.Name).IsClustered(false).IsUnique();

            #region Relationships
            builder.HasMany(status => status.Orders).WithOne(order => order.Status);
            #endregion
        }
    }
}
