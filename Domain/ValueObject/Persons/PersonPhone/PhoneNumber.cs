using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.PersonPhone
{
    public record PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty.");

            if (value.Length > 30)
                throw new ArgumentException("Phone number cannot exceed 30 characters.");

            if (!value.All(c => char.IsDigit(c) || c == '-' || c == ' '))
                throw new ArgumentException("Phone number can only contain digits, dashes or spaces.");

            Value = value.Trim();
        }
    }
}