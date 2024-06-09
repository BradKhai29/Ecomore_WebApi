using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class ProductImageEntityConfiguration : IEntityConfiguration<ProductImageEntity>
    {
        public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
        {
            builder.ToTable(ProductImageEntity.MetaData.TableName);

            builder.HasKey(image => image.Id);

            builder.Property(image => image.ProductId).IsRequired();

            builder.Property(image => image.UploadOrder).IsRequired();

            builder
                .Property(image => image.FileName)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_50)
                .IsRequired();

            builder
                .Property(image => image.StorageUrl)
                .HasColumnType(SqlDataTypes.SqlServer.VARCHAR_200)
                .IsRequired();
        }
    }
}
