using DataAccess.Commons.SqlConstants;
using DataAccess.Configurations.Base;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    internal class PaymentMethodEntityConfiguration : IEntityConfiguration<PaymentMethodEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentMethodEntity> builder)
        {
            builder.ToTable(PaymentMethodEntity.MetaData.TableName);

            builder.HasKey(paymentMethod => paymentMethod.Id);

            builder
                .Property(paymentMethod => paymentMethod.Name)
                .HasColumnType(SqlDataTypes.SqlServer.NVARCHAR_20)
                .IsRequired();

            builder.HasIndex(paymentMethod => paymentMethod.Name).IsUnique();

            #region Relationships
            builder
                .HasMany(paymentMethod => paymentMethod.Orders)
                .WithOne(order => order.PaymentMethod);
            #endregion
        }
    }
}
