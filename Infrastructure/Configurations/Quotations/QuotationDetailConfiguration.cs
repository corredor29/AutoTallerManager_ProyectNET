using Domain.Entities.Quotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotations;

public sealed class QuotationDetailConfiguration : IEntityTypeConfiguration<QuotationDetail>
{
    public void Configure(EntityTypeBuilder<QuotationDetail> builder)
    {
        builder.ToTable("quotation_details");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuotationId)
            .HasColumnName("quotation_id")
            .IsRequired();

        builder.Property(x => x.PartId)
            .HasColumnName("part_id")
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

        builder.HasOne(x => x.Quotation)
            .WithMany(x => x.Details)
            .HasForeignKey(x => x.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Part)
            .WithMany(x => x.QuotationDetails)
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.QuotationId, x.PartId })
            .IsUnique();

        builder.HasIndex(x => x.PartId);
    }
}
