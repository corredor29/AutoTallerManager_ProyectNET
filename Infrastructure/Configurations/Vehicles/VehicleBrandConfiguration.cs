using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles;

public sealed class VehicleBrandConfiguration : IEntityTypeConfiguration<VehicleBrand>
{
    public void Configure(EntityTypeBuilder<VehicleBrand> builder)
    {
        builder.ToTable("vehicle_brands");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BrandName)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(100)
            .HasColumnName("brand_name")
            .IsRequired();

        builder.HasIndex(x => x.BrandName)
            .IsUnique();
    }
}
