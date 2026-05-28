using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.DocumentType
{
    public record DocumentTypeName
    {
        public string Value { get; }

        public DocumentTypeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Document type name cannot be empty.");

            if (value.Length > 80)
                throw new ArgumentException("Document type name cannot exceed 80 characters.");

            Value = value.Trim();
        }
    }
}