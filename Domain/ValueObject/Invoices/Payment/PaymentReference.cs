using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.Payment
{
    public record PaymentReference
    {
        public string? Value { get; }

        public PaymentReference(string? value)
        {
            if (value is not null && value.Length > 100)
                throw new ArgumentException("Payment reference cannot exceed 100 characters.");

            if (value is not null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Payment reference cannot be empty if provided.");

            Value = value?.Trim();
        }
    }
}