using Domain.Entities.Parts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Parts;

public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.ToTable("parts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PartCategoryId)
            .HasColumnName("part_category_id")
            .IsRequired();

        builder.Property(x => x.UnitId)
            .HasColumnName("unit_id");

        builder.Property(x => x.Code)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(50)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(255)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(x => x.Stock)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("stock")
            .IsRequired();

        builder.Property(x => x.MinStock)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("min_stock")
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnType("numeric(18,2)")
            .HasColumnName("unit_price")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.PartCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.PartCategoryId);
        builder.HasIndex(x => x.UnitId);

        builder.Ignore(x => x.PartSuppliers);
    }
}
