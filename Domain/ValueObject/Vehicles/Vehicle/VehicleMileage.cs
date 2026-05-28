using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.Vehicle
{
    public record VehicleMileage
    {
        public int Value { get; }

        public VehicleMileage(int value)
        {
            if (value < 0)
                throw new ArgumentException("Mileage cannot be negative.");

            Value = value;
        }
    }
}