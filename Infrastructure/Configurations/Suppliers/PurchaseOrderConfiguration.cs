using Domain.Entities.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Suppliers;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SupplierId)
            .HasColumnName("supplier_id")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.PurchaseOrderStatusId)
            .HasColumnName("purchase_order_status_id")
            .IsRequired();

        builder.Property(x => x.OrderedAt)
            .HasColumnName("ordered_at")
            .IsRequired();

        builder.Property(x => x.ReceivedAt)
            .HasColumnName("received_at");

        builder.Property(x => x.Total)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("total")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(500)
            .HasColumnName("notes")
            .IsRequired(false);

        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.PurchaseOrders)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany(x => x.PurchaseOrders)
            .HasForeignKey(x => x.PurchaseOrderStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SupplierId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PurchaseOrderStatusId);
    }
}
