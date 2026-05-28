using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.VehicleModel
{
    public record ModelName
    {
        public string Value { get; }

        public ModelName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Model name cannot be empty.");

            if (value.Length > 80)
                throw new ArgumentException("Model name cannot exceed 80 characters.");

            Value = value.Trim();
        }
    }
}