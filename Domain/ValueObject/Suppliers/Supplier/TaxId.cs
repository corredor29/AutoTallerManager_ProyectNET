using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.Supplier
{
    public record TaxId
    {
        public string? Value { get; }

        public TaxId(string? value)
        {
            if (value is not null && value.Length > 50)
                throw new ArgumentException("Tax ID cannot exceed 50 characters.");

            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tax ID cannot be empty if provided.");

            Value = value?.Trim();
        }
    }
}