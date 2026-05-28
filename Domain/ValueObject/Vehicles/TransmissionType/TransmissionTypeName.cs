using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.TransmissionType
{
    public record TransmissionTypeName
    {
        public string Value { get; }

        public TransmissionTypeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Transmission type name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Transmission type name cannot exceed 50 characters.");

            Value = value.Trim();
        }
    }
}