using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.Supplier
{
    public record SupplierEmail
    {
        public string? Value { get; }

        public SupplierEmail(string? value)
        {
            if (value is not null && value.Length > 150)
                throw new ArgumentException("Email cannot exceed 150 characters.");

            if (value is not null && !value.Contains('@'))
                throw new ArgumentException("Email must contain '@'.");

            Value = value?.ToLower().Trim();
        }
    }
}