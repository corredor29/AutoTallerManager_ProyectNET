using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.InvoiceDetail
{
    public record InvoiceDetailDescription
    {
        public string Value { get; }

        public InvoiceDetailDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be empty.");

            if (value.Length > 150)
                throw new ArgumentException("Description cannot exceed 150 characters.");

            Value = value.Trim();
        }
    }
}