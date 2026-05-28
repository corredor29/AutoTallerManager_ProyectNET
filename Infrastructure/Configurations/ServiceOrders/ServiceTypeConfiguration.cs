using Domain.Entities.ServiceOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.ServiceOrders;

public sealed class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> builder)
    {
        builder.ToTable("service_types");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(80)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(x => x.EstimatedDuration)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("estimated_duration_hours");

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}
