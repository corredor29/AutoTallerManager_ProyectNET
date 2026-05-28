using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Persons.DocumentType
{
    public record DocumentCode
    {
        public string Value { get; }

        public DocumentCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Document code cannot be empty.");

            if (value.Length > 10)
                throw new ArgumentException("Document code cannot exceed 10 characters.");

            Value = value.ToUpper().Trim();
        }
    }

}