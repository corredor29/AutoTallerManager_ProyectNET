using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons
{
    public sealed class PersonEmailConfiguration : IEntityTypeConfiguration<PersonEmail>
    {
        public void Configure(EntityTypeBuilder<PersonEmail> builder)
        {
            builder.ToTable("PersonEmails");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PersonId)
                .IsRequired();

            builder.Property(x => x.EmailDomainId)
                .IsRequired();

            builder.Property(x => x.EmailUser)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => new { x.EmailUser, x.EmailDomainId })
                .IsUnique();

            builder.HasOne(x => x.Person)
                .WithMany(x => x.Emails)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.EmailDomain)
                .WithMany(x => x.PersonEmails)
                .HasForeignKey(x => x.EmailDomainId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}