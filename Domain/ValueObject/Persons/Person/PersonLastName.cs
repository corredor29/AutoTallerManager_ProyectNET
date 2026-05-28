using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.Person
{
    public record PersonLastName
    {
        public string Value { get; }

        public PersonLastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Last name cannot be empty.");

            if (value.Length > 100)
                throw new ArgumentException("Last name cannot exceed 100 characters.");

            Value = value.Trim();
        }
    }
}