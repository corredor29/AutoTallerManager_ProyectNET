using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.QuotationDetail
{
    public record QuotationQuantity
    {
        public int Value { get; }

        public QuotationQuantity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than 0.");

            Value = value;
        }
    }
}