using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.ServiceOrders.ServiceOrder
{
    public record WorkDescription
    {
        public string? Value { get; }

        public WorkDescription(string? value)
        {
            if (value is not null && value.Length > 2000)
                throw new ArgumentException("Work description cannot exceed 2000 characters.");

            Value = value?.Trim();
        }
    }
}