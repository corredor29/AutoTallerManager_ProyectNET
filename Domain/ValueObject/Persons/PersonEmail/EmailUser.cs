using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.PersonEmail
{

    public record EmailUser
    {
        public string Value { get; }

        public EmailUser(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email user cannot be empty.");

            if (value.Length > 100)
                throw new ArgumentException("Email user cannot exceed 100 characters.");

            if (value.Contains('@'))
                throw new ArgumentException("Email user must not contain '@'. Only the part before '@'.");

            if (value.Contains(' '))
                throw new ArgumentException("Email user cannot contain spaces.");

            Value = value.ToLower().Trim();
        }
    }
}