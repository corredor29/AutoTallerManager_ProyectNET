using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.Invoice
{
    public record InvoiceTax
    {
        public decimal Value { get; }

        public InvoiceTax(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Tax cannot be negative.");

            Value = value;
        }
    }
}