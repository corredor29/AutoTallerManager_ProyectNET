using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.MeasurementUnit
{
    public record MeasurementUnitAbbreviation
    {
        public string Value { get; }

        public MeasurementUnitAbbreviation(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Abbreviation cannot be empty.");

            if (value.Length > 10)
                throw new ArgumentException("Abbreviation cannot exceed 10 characters.");

            Value = value.ToLower().Trim();
        }
    }
}