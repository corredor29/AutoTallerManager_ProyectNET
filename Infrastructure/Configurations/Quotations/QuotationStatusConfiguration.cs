using Domain.Entities.Quotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Quotations;

public sealed class QuotationStatusConfiguration : IEntityTypeConfiguration<QuotationStatus>
{
    public void Configure(EntityTypeBuilder<QuotationStatus> builder)
    {
        builder.ToTable("quotation_statuses");

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
