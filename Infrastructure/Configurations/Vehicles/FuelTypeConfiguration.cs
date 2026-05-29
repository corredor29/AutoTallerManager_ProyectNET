using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles;

public sealed class FuelTypeConfiguration : IEntityTypeConfiguration<FuelType>
{
    public void Configure(EntityTypeBuilder<FuelType> builder)
    {
        builder.ToTable("fuel_types");

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
