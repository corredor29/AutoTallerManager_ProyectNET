using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.Vehicle
{
    public record VehicleYear
    {
        public short Value { get; }

        public VehicleYear(short value)
        {
            if (value < 1886)
                throw new ArgumentException("Vehicle year cannot be before 1886.");

            if (value > DateTime.UtcNow.Year + 1)
                throw new ArgumentException($"Vehicle year cannot exceed {DateTime.UtcNow.Year + 1}.");

            Value = value;
        }
    }
}