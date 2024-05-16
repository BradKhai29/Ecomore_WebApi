using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class ProductEntityConfiguration : IEntityConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.ToTable(ProductEntity.MetaData.TableName);

            builder.HasKey(product => product.Id);

            builder.Property(product => product.CategoryId).IsRequired();

            builder
                .Property(product => product.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_200)
                .IsRequired();

            builder
                .Property(product => product.UnitPrice)
                .HasColumnType(SqlDataTypes.SqlServer.DECIMAL_12_02)
                .IsRequired();

            builder
                .Property(product => product.Description)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_500)
                .IsRequired();

            builder.Property(product => product.ProductStatusId).IsRequired();

            builder.Property(product => product.QuantityInStock).IsRequired();

            builder
                .Property(product => product.CreatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder.Property(product => product.CreatedBy).IsRequired();

            builder
                .Property(product => product.UpdatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME)
                .IsRequired();

            builder.Property(product => product.UpdatedBy).IsRequired();

            #region Relationships
            builder.HasMany(product => product.ProductImages).WithOne(image => image.Product);

            builder
                .HasMany(product => product.OrderItems)
                .WithOne(orderItem => orderItem.Product)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
