using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.Quotation
{
    public record QuotationSubtotal
    {
        public decimal Value { get; }

        public QuotationSubtotal(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Subtotal cannot be negative.");

            Value = value;
        }
    }
}