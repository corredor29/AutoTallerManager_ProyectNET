using Domain.Entities.ServiceOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.ServiceOrders;

public sealed class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.ToTable("service_orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();

        builder.Property(x => x.ServiceTypeId)
            .HasColumnName("service_type_id")
            .IsRequired();

        builder.Property(x => x.MechanicId)
            .HasColumnName("mechanic_id")
            .IsRequired();

        builder.Property(x => x.OrderStatusId)
            .HasColumnName("order_status_id")
            .IsRequired();

        builder.Property(x => x.AppointmentId)
            .HasColumnName("appointment_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.EstimatedDeliveryAt)
            .HasColumnName("estimated_delivery_at");

        builder.Property(x => x.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(x => x.WorkPerformed)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(2000)
            .HasColumnName("work_performed");

        builder.Property(x => x.Notes)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(500)
            .HasColumnName("notes");

        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.ServiceOrders)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ServiceType)
            .WithMany(x => x.ServiceOrders)
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Mechanic)
            .WithMany()
            .HasForeignKey(x => x.MechanicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OrderStatus)
            .WithMany(x => x.ServiceOrders)
            .HasForeignKey(x => x.OrderStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Appointment)
            .WithOne(x => x.ServiceOrder)
            .HasForeignKey<ServiceOrder>(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.VehicleId);
        builder.HasIndex(x => x.OrderStatusId);
        builder.HasIndex(x => new { x.VehicleId, x.ClosedAt });

        builder.Ignore(x => x.Parts);
        builder.Ignore(x => x.Quotations);
        builder.Ignore(x => x.Invoice);
    }
}
