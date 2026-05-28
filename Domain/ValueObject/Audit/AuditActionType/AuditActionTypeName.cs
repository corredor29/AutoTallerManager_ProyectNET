using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Audit.AuditActionType
{

    public record AuditActionTypeName
    {
        public string Value { get; }

        public AuditActionTypeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Audit action type name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Audit action type name cannot exceed 50 characters.");

            if (value is not ("Create" or "Update" or "Delete" or "Login" or "Logout"))
                throw new ArgumentException("Audit action type must be Create, Update, Delete, Login or Logout.");

            Value = value.Trim();
        }
    } 
}