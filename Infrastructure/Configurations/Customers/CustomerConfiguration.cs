using Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Customers;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PersonId)
            .HasColumnName("person_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(x => x.Value, value => new(value))
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(x => x.PersonId)
            .IsUnique();

        builder.HasOne(x => x.Person)
            .WithOne(x => x.Customer)
            .HasForeignKey<Customer>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.Ownerships);
    }
}
