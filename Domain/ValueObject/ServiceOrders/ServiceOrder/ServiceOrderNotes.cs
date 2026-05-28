using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.ServiceOrders.ServiceOrder
{
    public record ServiceOrderNotes
    {
        public string? Value { get; }

        public ServiceOrderNotes(string? value)
        {
            if (value is not null && value.Length > 500)
                throw new ArgumentException("Service order notes cannot exceed 500 characters.");

            Value = value?.Trim();
        }
    }
}