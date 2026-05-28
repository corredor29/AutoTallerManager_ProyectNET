using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.Supplier
{
    public record SupplierPhone
    {
        public string? Value { get; }

        public SupplierPhone(string? value)
        {
            if (value is not null && value.Length > 30)
                throw new ArgumentException("Phone cannot exceed 30 characters.");

            if (value is not null && !value.All(c => char.IsDigit(c) || c == '-' || c == '+' || c == ' '))
                throw new ArgumentException("Phone can only contain digits, dashes, plus or spaces.");

            Value = value?.Trim();
        }
    }
}