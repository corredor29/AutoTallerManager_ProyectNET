using Domain.Entities.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Suppliers;

public sealed class PurchaseOrderStatusConfiguration : IEntityTypeConfiguration<PurchaseOrderStatus>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderStatus> builder)
    {
        builder.ToTable("purchase_order_statuses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(50)
            .HasColumnName("name")
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
