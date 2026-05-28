using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.FuelType
{
    public record FuelTypeName
    {
        public string Value { get; }

        public FuelTypeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Fuel type name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Fuel type name cannot exceed 50 characters.");

            Value = value.Trim();
        }
    }
}