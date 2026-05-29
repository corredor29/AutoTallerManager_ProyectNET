using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles;

public sealed class TransmissionTypeConfiguration : IEntityTypeConfiguration<TransmissionType>
{
    public void Configure(EntityTypeBuilder<TransmissionType> builder)
    {
        builder.ToTable("transmission_types");

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
