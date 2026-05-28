using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.Part
{
    public record PartMinStock
    {
        public int Value { get; }

        public PartMinStock(int value)
        {
            if (value < 0)
                throw new ArgumentException("Minimum stock cannot be negative.");

            Value = value;
        }
    }
}