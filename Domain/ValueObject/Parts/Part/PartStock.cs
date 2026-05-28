using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.Part
{
    public record PartStock
    {
        public int Value { get; }

        public PartStock(int value)
        {
            if (value < 0)
                throw new ArgumentException("Stock cannot be negative.");

            Value = value;
        }

        public bool IsEmpty => Value == 0;
    }
}