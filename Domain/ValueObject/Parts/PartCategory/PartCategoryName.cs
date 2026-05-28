using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Parts.PartCategory
{
    public record PartCategoryName
    {
        public string Value { get; }

        public PartCategoryName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Part category name cannot be empty.");

            if (value.Length > 80)
                throw new ArgumentException("Part category name cannot exceed 80 characters.");

            Value = value.Trim();
        }
    }
}