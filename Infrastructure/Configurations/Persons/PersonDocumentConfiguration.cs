using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Persons
{
    public sealed class PersonDocumentConfiguration : IEntityTypeConfiguration<PersonDocument>
    {
        public void Configure(EntityTypeBuilder<PersonDocument> builder)
        {
            builder.ToTable("PersonDocuments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PersonId)
                .IsRequired();

            builder.Property(x => x.DocumentTypeId)
                .IsRequired();

            builder.Property(x => x.DocumentNumber)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => new { x.DocumentTypeId, x.DocumentNumber })
                .IsUnique();

            builder.HasOne(x => x.Person)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.DocumentType)
                .WithMany(x => x.PersonDocuments)
                .HasForeignKey(x => x.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}