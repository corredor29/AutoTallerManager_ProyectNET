using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Audit
{
    public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.AuditActionTypeId)
                .IsRequired();

            builder.Property(x => x.AffectedEntity)
                .HasConversion(v => v.Value, v => new(v))
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.AffectedRecordId)
                .IsRequired();

            builder.Property(x => x.OccurredAt)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasConversion(v => v.Value, v => new(v))
                .IsRequired(false);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.OccurredAt);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AuditActionType)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.AuditActionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}