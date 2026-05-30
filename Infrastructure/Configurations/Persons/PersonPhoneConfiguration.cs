using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons
{
    public sealed class PersonPhoneConfiguration : IEntityTypeConfiguration<PersonPhone>
    {
        public void Configure(EntityTypeBuilder<PersonPhone> builder)
        {
            builder.ToTable("PersonPhones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PersonId)
                .IsRequired();

            builder.Property(x => x.PhoneCodeId)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => new { x.PhoneCodeId, x.PhoneNumber })
                .IsUnique();

            builder.HasOne(x => x.Person)
                .WithMany(x => x.Phones)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PhoneCode)
                .WithMany(x => x.PersonPhones)
                .HasForeignKey(x => x.PhoneCodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}