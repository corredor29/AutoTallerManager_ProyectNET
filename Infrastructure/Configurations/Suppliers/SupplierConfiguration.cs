using Domain.Entities.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Suppliers;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyName)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(150)
            .HasColumnName("company_name")
            .IsRequired();

        builder.Property(x => x.TaxId)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(50)
            .HasColumnName("tax_id")
            .IsRequired(false);

        builder.Property(x => x.ContactName)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(100)
            .HasColumnName("contact_name")
            .IsRequired(false);

        builder.Property(x => x.Phone)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(30)
            .HasColumnName("phone")
            .IsRequired(false);

        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(150)
            .HasColumnName("email")
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("address")
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(x => x.TaxId)
            .IsUnique();

        builder.Ignore(x => x.PurchaseOrders);
    }
}
