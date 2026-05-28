using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.VehicleBrand
{
    public record BrandName
    {
        public string Value { get; }

        public BrandName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Brand name cannot be empty.");

            if (value.Length > 80)
                throw new ArgumentException("Brand name cannot exceed 80 characters.");

            Value = value.Trim();
        }
    }
}