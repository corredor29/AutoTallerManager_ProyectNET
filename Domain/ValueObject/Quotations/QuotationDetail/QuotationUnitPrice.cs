using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.QuotationDetail
{
    public record QuotationUnitPrice
    {
        public decimal Value { get; }

        public QuotationUnitPrice(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            Value = value;
        }
    }
}