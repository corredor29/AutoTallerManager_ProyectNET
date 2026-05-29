using Domain.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Invoices;

public sealed class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
{
    public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
    {
        builder.ToTable("invoice_details");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InvoiceId)
            .HasColumnName("invoice_id")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(150)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("unit_price")
            .IsRequired();

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.InvoiceId);
    }
}
