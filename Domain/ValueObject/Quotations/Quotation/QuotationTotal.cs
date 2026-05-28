using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.Quotation
{
    public record QuotationTotal
    {
        public decimal Value { get; }

        public QuotationTotal(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Total cannot be negative.");

            Value = value;
        }
    }
}