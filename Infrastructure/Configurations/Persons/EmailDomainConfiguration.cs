using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons
{
    public sealed class EmailDomainConfiguration : IEntityTypeConfiguration<EmailDomain>
    {
        public void Configure(EntityTypeBuilder<EmailDomain> builder)
        {
            builder.ToTable("EmailDomains");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Domain)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.Domain).IsUnique();

            builder.HasMany(x => x.PersonEmails)
                .WithOne(x => x.EmailDomain)
                .HasForeignKey(x => x.EmailDomainId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}