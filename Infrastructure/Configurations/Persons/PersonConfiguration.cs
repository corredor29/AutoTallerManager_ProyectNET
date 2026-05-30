using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Domain.Entities.Customers;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons
{
    public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.RegisteredAt)
                .IsRequired();

            builder.HasOne(x => x.Customer)
                .WithOne(x => x.Person)
                .HasForeignKey<Customer>(x => x.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                .WithOne(x => x.Person)
                .HasForeignKey<User>(x => x.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Documents)
                .WithOne(x => x.Person)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Emails)
                .WithOne(x => x.Person)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Phones)
                .WithOne(x => x.Person)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}