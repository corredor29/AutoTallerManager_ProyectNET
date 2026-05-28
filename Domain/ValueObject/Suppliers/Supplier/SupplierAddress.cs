using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.Supplier
{
    public record SupplierAddress
    {
        public string? Value { get; }

        public SupplierAddress(string? value)
        {
            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Address cannot be empty if provided.");

            Value = value?.Trim();
        }
    }
}