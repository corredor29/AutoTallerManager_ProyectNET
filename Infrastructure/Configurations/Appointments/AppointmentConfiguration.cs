using Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Appointments;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(x => x.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();

        builder.Property(x => x.ServiceTypeId)
            .HasColumnName("service_type_id")
            .IsRequired();

        builder.Property(x => x.AppointmentStatusId)
            .HasColumnName("appointment_status_id")
            .IsRequired();

        builder.Property(x => x.AssignedUserId)
            .HasColumnName("assigned_user_id");

        builder.Property(x => x.AppointmentDate)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("appointment_date")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(500)
            .HasColumnName("notes");

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ServiceType)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AppointmentStatus)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.AppointmentStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
