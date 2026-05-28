using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.PhoneCode
{

    public record PhoneCodeCountry
    {
        public string Value { get; }

        public PhoneCodeCountry(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Country name cannot be empty.");

            if (value.Length > 80)
                throw new ArgumentException("Country name cannot exceed 80 characters.");

            Value = value.Trim();
        }
    }
}