using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.Quotation
{
    public record LaborCost
    {
        public decimal Value { get; }

        public LaborCost(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Labor cost cannot be negative.");

            Value = value;
        }
    }
}