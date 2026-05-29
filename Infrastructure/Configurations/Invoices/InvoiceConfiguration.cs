using Domain.Entities.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Invoices;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceOrderId)
            .HasColumnName("service_order_id")
            .IsRequired();

        builder.Property(x => x.QuotationId)
            .HasColumnName("quotation_id");

        builder.Property(x => x.IssuedAt)
            .HasColumnName("issued_at")
            .IsRequired();

        builder.Property(x => x.LaborCost)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("labor_cost")
            .IsRequired();

        builder.Property(x => x.Subtotal)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("subtotal")
            .IsRequired();

        builder.Property(x => x.Tax)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("tax")
            .IsRequired();

        builder.Property(x => x.Total)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("total")
            .IsRequired();

        builder.Property(x => x.DiagnosisOnlyCharged)
            .HasColumnName("diagnosis_only_charged")
            .IsRequired();

        builder.HasOne(x => x.ServiceOrder)
            .WithOne(x => x.Invoice)
            .HasForeignKey<Invoice>(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quotation)
            .WithOne(x => x.Invoice)
            .HasForeignKey<Invoice>(x => x.QuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ServiceOrderId)
            .IsUnique();

        builder.HasIndex(x => x.QuotationId)
            .IsUnique();

        builder.Ignore(x => x.Payments);
    }
}
