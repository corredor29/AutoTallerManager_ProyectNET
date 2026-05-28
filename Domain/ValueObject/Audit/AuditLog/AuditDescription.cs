using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Audit.AuditLog
{
    public record AuditDescription
    {
        public string? Value { get; }

        public AuditDescription(string? value)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Audit description cannot be empty if provided.");

            Value = value?.Trim();
        }
    }
}