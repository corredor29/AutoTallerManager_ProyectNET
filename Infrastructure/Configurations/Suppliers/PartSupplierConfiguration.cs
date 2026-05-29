using Domain.Entities.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Suppliers;

public sealed class PartSupplierConfiguration : IEntityTypeConfiguration<PartSupplier>
{
    public void Configure(EntityTypeBuilder<PartSupplier> builder)
    {
        builder.ToTable("part_suppliers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PartId)
            .HasColumnName("part_id")
            .IsRequired();

        builder.Property(x => x.SupplierId)
            .HasColumnName("supplier_id")
            .IsRequired();

        builder.Property(x => x.PurchasePrice)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("purchase_price")
            .IsRequired();

        builder.Property(x => x.IsPrimary)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("is_primary")
            .IsRequired();

        builder.HasOne(x => x.Part)
            .WithMany(x => x.PartSuppliers)
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.PartSuppliers)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.PartId, x.SupplierId })
            .IsUnique();

        builder.HasIndex(x => x.SupplierId);
    }
}
