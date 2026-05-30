using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles
{

    public sealed class VehicleColorConfiguration : IEntityTypeConfiguration<VehicleColor>
    {
        public void Configure(EntityTypeBuilder<VehicleColor> builder)
        {
            builder.ToTable("VehicleColors");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.Vehicles)
                .WithOne(x => x.Color)
                .HasForeignKey(x => x.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}