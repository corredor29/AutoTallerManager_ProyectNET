using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.PhoneCode
{
    public record PhoneCodeValue
    {
        public string Value { get; }

        public PhoneCodeValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone code cannot be empty.");

            if (value.Length > 10)
                throw new ArgumentException("Phone code cannot exceed 10 characters.");

            if (!value.StartsWith("+"))
                throw new ArgumentException("Phone code must start with '+'. Example: +57");

            Value = value.Trim();
        }
    }
}