using Domain.Entities.Quotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotations;

public sealed class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("quotations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceOrderId)
            .HasColumnName("service_order_id")
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();

        builder.Property(x => x.QuotationStatusId)
            .HasColumnName("quotation_status_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.RespondedAt)
            .HasColumnName("responded_at");

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

        builder.Property(x => x.Total)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("total")
            .IsRequired();

        builder.Property(x => x.RejectionReason)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(500)
            .HasColumnName("rejection_reason")
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(500)
            .HasColumnName("notes")
            .IsRequired(false);

        builder.HasOne(x => x.ServiceOrder)
            .WithMany(x => x.Quotations)
            .HasForeignKey(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.QuotationStatus)
            .WithMany(x => x.Quotations)
            .HasForeignKey(x => x.QuotationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ServiceOrderId);
        builder.HasIndex(x => x.CreatedByUserId);
        builder.HasIndex(x => x.QuotationStatusId);

        builder.Ignore(x => x.Details);
        builder.Ignore(x => x.Invoice);
    }
}
