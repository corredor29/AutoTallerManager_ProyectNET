using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles;

public sealed class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("vehicle_models");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BrandId)
            .HasColumnName("brand_id")
            .IsRequired();

        builder.Property(x => x.ModelName)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(100)
            .HasColumnName("model_name")
            .IsRequired();

        builder.HasIndex(x => new { x.BrandId, x.ModelName })
            .IsUnique();

        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Models)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
