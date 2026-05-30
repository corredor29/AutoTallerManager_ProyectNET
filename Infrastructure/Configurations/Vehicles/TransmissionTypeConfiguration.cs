using Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Vehicles
{
    public sealed class TransmissionTypeConfiguration : IEntityTypeConfiguration<TransmissionType>
    {
        public void Configure(EntityTypeBuilder<TransmissionType> builder)
        {
            builder.ToTable("TransmissionTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.Vehicles)
                .WithOne(x => x.TransmissionType)
                .HasForeignKey(x => x.TransmissionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}