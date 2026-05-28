using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.InvoiceDetail
{
    public record InvoiceDetailUnitPrice
    {
        public decimal Value { get; }

        public InvoiceDetailUnitPrice(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            Value = value;
        }
    }
}