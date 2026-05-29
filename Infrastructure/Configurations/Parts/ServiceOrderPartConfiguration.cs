using Domain.Entities.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Parts;

public sealed class ServiceOrderPartConfiguration : IEntityTypeConfiguration<ServiceOrderPart>
{
    public void Configure(EntityTypeBuilder<ServiceOrderPart> builder)
    {
        builder.ToTable("service_order_parts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceOrderId)
            .HasColumnName("service_order_id")
            .IsRequired();

        builder.Property(x => x.PartId)
            .HasColumnName("part_id")
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.AppliedUnitPrice)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("applied_unit_price")
            .IsRequired();

        builder.HasOne(x => x.ServiceOrder)
            .WithMany(x => x.Parts)
            .HasForeignKey(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Part)
            .WithMany(x => x.OrderParts)
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ServiceOrderId, x.PartId })
            .IsUnique();

        builder.HasIndex(x => x.PartId);
    }
}
