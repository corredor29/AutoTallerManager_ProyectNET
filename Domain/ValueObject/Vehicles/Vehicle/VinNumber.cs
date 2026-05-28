using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.Vehicle
{
    public record VinNumber
    {
        public string Value { get; }

        public VinNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("VIN cannot be empty.");

            if (value.Length != 17)
                throw new ArgumentException("VIN must be exactly 17 characters.");

            if (!value.All(char.IsLetterOrDigit))
                throw new ArgumentException("VIN can only contain letters and digits.");

            Value = value.ToUpper().Trim();
        }
    }
}