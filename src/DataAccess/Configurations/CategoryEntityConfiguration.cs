using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class CategoryEntityConfiguration : IEntityConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable(CategoryEntity.MetaData.TableName);

            builder.HasKey(category => category.Id);

            builder
                .Property(category => category.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_50)
                .IsRequired();

            builder.HasIndex(category => category.Name).IsUnique();

            builder
                .Property(category => category.CreatedAt)
                .HasColumnType(SqlDataTypes.SqlServer.DATETIME);

            builder.Property(category => category.CreatedBy).IsRequired();

            #region Relationships
            builder
                .HasMany(category => category.Products)
                .WithOne(product => product.Category)
                .HasPrincipalKey(category => category.Id)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(category => category.Creator)
                .WithMany(creator => creator.CreatedCategories)
                .HasPrincipalKey(creator => creator.Id)
                .HasForeignKey(category => category.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
