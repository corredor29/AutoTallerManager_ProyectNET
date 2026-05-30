using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles;

public sealed class FuelTypeConfiguration : IEntityTypeConfiguration<FuelType>
{
    public void Configure(EntityTypeBuilder<FuelType> builder)
    {
        builder.ToTable("FuelTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasConversion(v => v.Value, v => new(v))
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Vehicles)
               .WithOne(x => x.FuelType)
               .HasForeignKey(x => x.FuelTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}