using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class ProductStatusEntityConfiguration : IEntityConfiguration<ProductStatusEntity>
    {
        public void Configure(EntityTypeBuilder<ProductStatusEntity> builder)
        {
            builder.ToTable(ProductStatusEntity.MetaData.TableName);

            builder.HasKey(status => status.Id);

            builder
                .Property(status => status.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_50)
                .IsRequired();

            builder.HasIndex(status => status.Name).IsUnique().IsClustered(false);

            #region Relationships Configuration.
            builder
                .HasMany(status => status.Products)
                .WithOne(product => product.ProductStatus)
                .HasPrincipalKey(status => status.Id)
                .HasForeignKey(product => product.ProductStatusId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
