using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.EmailDomain
{
    public record EmailDomainValue
    {
        public string Value { get; }

        public EmailDomainValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email domain cannot be empty.");

            if (value.Length > 100)
                throw new ArgumentException("Email domain cannot exceed 100 characters.");

            if (!value.Contains('.'))
                throw new ArgumentException("Email domain must contain a dot. Example: gmail.com");

            Value = value.ToLower().Trim();
        }
    }
}