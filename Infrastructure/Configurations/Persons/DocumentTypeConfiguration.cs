using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Configurations.Persons
{
    public sealed class DocumentTypeConfiguration : IEntityTypeConfiguration<DocumentType>
    {
        public void Configure(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("DocumentTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(80)
                .IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.PersonDocuments)
                .WithOne(x => x.DocumentType)
                .HasForeignKey(x => x.DocumentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}