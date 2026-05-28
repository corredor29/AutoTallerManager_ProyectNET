using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.Part
{
    public record PartCode
    {
        public string Value { get; }

        public PartCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Part code cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Part code cannot exceed 50 characters.");

            Value = value.ToUpper().Trim();
        }
    }
}