using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Audit.AuditLog
{
    public record AffectedEntityName
    {
        public string Value { get; }

        public AffectedEntityName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Affected entity name cannot be empty.");

            if (value.Length > 100)
                throw new ArgumentException("Affected entity name cannot exceed 100 characters.");

            Value = value.Trim();
        }
    }
}