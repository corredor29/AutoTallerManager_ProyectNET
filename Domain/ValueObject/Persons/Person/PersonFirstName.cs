using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.Person
{
    public record PersonFirstName
    {
        public string Value { get; }

        public PersonFirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("First name cannot be empty.");

            if (value.Length > 100)
                throw new ArgumentException("First name cannot exceed 100 characters.");

            Value = value.Trim();
        }
    }
}