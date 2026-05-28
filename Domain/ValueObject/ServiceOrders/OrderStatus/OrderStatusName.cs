using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.ServiceOrders.OrderStatus
{
    public record OrderStatusName
    {
        public string Value { get; }

        public OrderStatusName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Order status name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Order status name cannot exceed 50 characters.");

            Value = value.Trim();
        }
    }
}