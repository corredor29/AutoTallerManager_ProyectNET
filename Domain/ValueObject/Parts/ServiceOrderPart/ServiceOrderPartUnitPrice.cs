using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.ServiceOrderPart
{
    public record ServiceOrderPartUnitPrice
    {
        public decimal Value { get; }

        public ServiceOrderPartUnitPrice(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            Value = value;
        }
    }
}