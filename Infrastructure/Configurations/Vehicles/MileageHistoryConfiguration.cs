using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles
{
    public sealed class MileageHistoryConfiguration : IEntityTypeConfiguration<MileageHistory>
    {
        public void Configure(EntityTypeBuilder<MileageHistory> builder)
        {
            builder.ToTable("MileageHistory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.VehicleId)
                .IsRequired();

            builder.Property(x => x.Mileage)
                .HasConversion(v => v.Value, v => new(v))
                .IsRequired();

            builder.Property(x => x.RecordedAt)
                .IsRequired();

            builder.Property(x => x.Notes)
                .IsRequired(false);

            builder.HasIndex(x => x.VehicleId);

            builder.HasOne(x => x.Vehicle)
                .WithMany(x => x.MileageHistories)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}