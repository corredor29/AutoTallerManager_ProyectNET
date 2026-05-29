using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons;

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(100)
            .HasColumnName("first_name")
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(100)
            .HasColumnName("last_name")
            .IsRequired();

        builder.Property(x => x.RegisteredAt)
            .HasColumnName("registered_at")
            .IsRequired();

        builder.Ignore(x => x.Documents);
        builder.Ignore(x => x.Emails);
        builder.Ignore(x => x.Phones);
    }
}
