using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.Invoice
{
    public record InvoiceSubtotal
    {
        public decimal Value { get; }

        public InvoiceSubtotal(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Subtotal cannot be negative.");

            Value = value;
        }
    }
}