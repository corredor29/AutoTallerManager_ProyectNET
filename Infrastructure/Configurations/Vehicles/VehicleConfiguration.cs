using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ModelId)
            .HasColumnName("model_id")
            .IsRequired();

        builder.Property(x => x.ColorId)
            .HasColumnName("color_id");

        builder.Property(x => x.FuelTypeId)
            .HasColumnName("fuel_type_id");

        builder.Property(x => x.TransmissionTypeId)
            .HasColumnName("transmission_type_id");

        builder.Property(x => x.VIN)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(17)
            .HasColumnName("vin")
            .IsRequired();

        builder.Property(x => x.Year)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("year")
            .IsRequired();

        builder.Property(x => x.Mileage)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("mileage")
            .IsRequired();

        builder.Property(x => x.LicensePlate)
            .HasConversion(
                x => x == null ? null : x.Value,
                value => string.IsNullOrWhiteSpace(value) ? null : new(value))
            .HasMaxLength(20)
            .HasColumnName("license_plate");

        builder.HasIndex(x => x.VIN)
            .IsUnique();

        builder.HasOne(x => x.Model)
            .WithMany(x => x.Vehicles)
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Color)
            .WithMany(x => x.Vehicles)
            .HasForeignKey(x => x.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FuelType)
            .WithMany(x => x.Vehicles)
            .HasForeignKey(x => x.FuelTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TransmissionType)
            .WithMany(x => x.Vehicles)
            .HasForeignKey(x => x.TransmissionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
