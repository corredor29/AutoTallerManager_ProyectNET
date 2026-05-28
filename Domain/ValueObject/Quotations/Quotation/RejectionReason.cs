using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Quotations.Quotation
{
    public record RejectionReason
    {
        public string? Value { get; }

        public RejectionReason(string? value)
        {
            if (value is not null && value.Length > 500)
                throw new ArgumentException("Rejection reason cannot exceed 500 characters.");

            Value = value?.Trim();
        }
    }
}