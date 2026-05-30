using Domain.Entities.ServiceOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.ServiceOrders
{

    public sealed class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.ToTable("OrderStatuses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.ServiceOrders)
                .WithOne(x => x.OrderStatus)
                .HasForeignKey(x => x.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}