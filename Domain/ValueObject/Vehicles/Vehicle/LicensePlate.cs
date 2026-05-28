using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.Vehicle
{
    public record LicensePlate
    {
        public string Value { get; }

        public LicensePlate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("License plate cannot be empty.");

            if (value.Length > 20)
                throw new ArgumentException("License plate cannot exceed 20 characters.");

            Value = value.ToUpper().Trim();
        }
    }
}