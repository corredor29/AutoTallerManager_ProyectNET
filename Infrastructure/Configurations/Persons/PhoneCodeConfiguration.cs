using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons
{
    public sealed class PhoneCodeConfiguration : IEntityTypeConfiguration<PhoneCode>
    {
        public void Configure(EntityTypeBuilder<PhoneCode> builder)
        {
            builder.ToTable("PhoneCodes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Country)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(80)
                .IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasMany(x => x.PersonPhones)
                .WithOne(x => x.PhoneCode)
                .HasForeignKey(x => x.PhoneCodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}