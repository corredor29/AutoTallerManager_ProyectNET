using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.Supplier
{
    public record SupplierContactName
    {
        public string? Value { get; }

        public SupplierContactName(string? value)
        {
            if (value is not null && value.Length > 100)
                throw new ArgumentException("Contact name cannot exceed 100 characters.");

            Value = value?.Trim();
        }
    }
}