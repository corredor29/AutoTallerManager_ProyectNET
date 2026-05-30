using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Audit
{
    public sealed class AuditActionTypeConfiguration : IEntityTypeConfiguration<AuditActionType>
    {
        public void Configure(EntityTypeBuilder<AuditActionType> builder)
        {
            builder.ToTable("AuditActionTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.AuditLogs)
                .WithOne(x => x.AuditActionType)
                .HasForeignKey(x => x.AuditActionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}