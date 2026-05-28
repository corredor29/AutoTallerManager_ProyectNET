using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.MileageHistory
{
    public record MileageRecord
    {
        public int Value { get; }

        public MileageRecord(int value)
        {
            if (value < 0)
                throw new ArgumentException("Mileage record cannot be negative.");

            Value = value;
        }
    }
}