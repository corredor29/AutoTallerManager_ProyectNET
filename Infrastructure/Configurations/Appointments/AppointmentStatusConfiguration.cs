using Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Appointments;

public sealed class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
{
    public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
    {
        builder.ToTable("appointment_statuses");

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
