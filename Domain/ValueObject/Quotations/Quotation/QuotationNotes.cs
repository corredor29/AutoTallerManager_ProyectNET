using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.Quotation
{
    public record QuotationNotes
    {
        public string? Value { get; }

        public QuotationNotes(string? value)
        {
            if (value is not null && value.Length > 500)
                throw new ArgumentException("Quotation notes cannot exceed 500 characters.");

            Value = value?.Trim();
        }
    }
}