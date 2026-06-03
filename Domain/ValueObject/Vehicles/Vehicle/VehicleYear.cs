using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.Vehicle
{
    public record VehicleYear
    {
        private const short MaxVehicleYear = 2026;

        public short Value { get; }

        public VehicleYear(short value)
        {
            if (value < 1886)
                throw new ArgumentException("Vehicle year cannot be before 1886.");

            if (value > MaxVehicleYear)
                throw new ArgumentException($"Vehicle year cannot exceed {MaxVehicleYear}.");

            Value = value;
        }
    }
}
