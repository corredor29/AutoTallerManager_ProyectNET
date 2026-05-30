using Domain.Entities.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Appointments
{
    public sealed class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
    {
        public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
        {
            builder.ToTable("AppointmentStatuses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.Appointments)
                .WithOne(x => x.AppointmentStatus)
                .HasForeignKey(x => x.AppointmentStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}