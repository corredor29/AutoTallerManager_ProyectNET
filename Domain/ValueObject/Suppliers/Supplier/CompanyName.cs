using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.Supplier
{
    public record CompanyName
    {
        public string Value { get; }

        public CompanyName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Company name cannot be empty.");

            if (value.Length > 150)
                throw new ArgumentException("Company name cannot exceed 150 characters.");

            Value = value.Trim();
        }
    }
}