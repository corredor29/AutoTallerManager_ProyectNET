using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.MeasurementUnit
{
    public record MeasurementUnitName
    {
        public string Value { get; }

        public MeasurementUnitName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Measurement unit name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Measurement unit name cannot exceed 50 characters.");

            Value = value.Trim();
        }
    }
}