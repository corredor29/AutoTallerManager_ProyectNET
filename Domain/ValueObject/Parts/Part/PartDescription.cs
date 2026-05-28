using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.Part
{
    public record PartDescription
    {
        public string Value { get; }

        public PartDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Part description cannot be empty.");

            if (value.Length > 255)
                throw new ArgumentException("Part description cannot exceed 255 characters.");

            Value = value.Trim();
        }
    }
}