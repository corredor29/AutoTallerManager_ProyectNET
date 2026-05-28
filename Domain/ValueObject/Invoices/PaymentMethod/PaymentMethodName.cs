using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.PaymentMethod
{
    public record PaymentMethodName
    {
        public string Value { get; }

        public PaymentMethodName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Payment method name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Payment method name cannot exceed 50 characters.");

            Value = value.Trim();
        }
    }
}