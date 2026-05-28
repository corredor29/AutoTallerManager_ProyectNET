using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.Invoice
{
    public record InvoiceTotal
    {
        public decimal Value { get; }

        public InvoiceTotal(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Invoice total cannot be negative.");

            Value = value;
        }
    }
}