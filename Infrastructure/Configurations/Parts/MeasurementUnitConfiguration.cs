using Domain.Entities.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Parts;

public sealed class MeasurementUnitConfiguration : IEntityTypeConfiguration<MeasurementUnit>
{
    public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder.ToTable("measurement_units");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(50)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(x => x.Abbreviation)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(10)
            .HasColumnName("abbreviation")
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasIndex(x => x.Abbreviation)
            .IsUnique();

        builder.Ignore(x => x.Parts);
    }
}
