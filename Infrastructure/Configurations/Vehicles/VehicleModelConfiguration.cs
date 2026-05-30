using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles
{

    public sealed class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
    {
        public void Configure(EntityTypeBuilder<VehicleModel> builder)
        {
            builder.ToTable("VehicleModels");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.BrandId)
                .IsRequired();

            builder.Property(x => x.ModelName)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(80)
                .IsRequired();

            builder.HasIndex(x => new { x.BrandId, x.ModelName })
                .IsUnique();

            builder.HasOne(x => x.Brand)
                .WithMany(x => x.Models)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Vehicles)
                .WithOne(x => x.Model)
                .HasForeignKey(x => x.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}