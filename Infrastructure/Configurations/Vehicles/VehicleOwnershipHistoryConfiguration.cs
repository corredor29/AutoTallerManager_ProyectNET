using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles
{
public sealed class VehicleOwnershipHistoryConfiguration : IEntityTypeConfiguration<VehicleOwnershipHistory>
{
    public void Configure(EntityTypeBuilder<VehicleOwnershipHistory> builder)
    {
        builder.ToTable("VehicleOwnershipHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.VehicleId)
               .IsRequired();

        builder.Property(x => x.CustomerId)
               .IsRequired();

        builder.OwnsOne(x => x.DateRange, dr =>
        {
            dr.Property(x => x.StartDate)
              .HasColumnName("StartDate")
              .IsRequired();

            dr.Property(x => x.EndDate)
              .HasColumnName("EndDate")
              .IsRequired(false);
        });

        builder.HasIndex(x => x.VehicleId);

        builder.HasOne(x => x.Vehicle)
               .WithMany(x => x.Ownerships)
               .HasForeignKey(x => x.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
               .WithMany(x => x.Ownerships)
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
}