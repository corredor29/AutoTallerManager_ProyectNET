using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.QuotationStatus
{
    public record QuotationStatusName
    {
        public string Value { get; }

        public QuotationStatusName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Quotation status name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Quotation status name cannot exceed 50 characters.");

            if (value is not ("Pending" or "Accepted" or "Rejected"))
                throw new ArgumentException("Quotation status must be Pending, Accepted or Rejected.");

            Value = value.Trim();
        }
    }
}