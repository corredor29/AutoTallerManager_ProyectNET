using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.Invoice
{
    public record InvoiceLaborCost
    {
        public decimal Value { get; }

        public InvoiceLaborCost(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Labor cost cannot be negative.");

            Value = value;
        }
    }
}