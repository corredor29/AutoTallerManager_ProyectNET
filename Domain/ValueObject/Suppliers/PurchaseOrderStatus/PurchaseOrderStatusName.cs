using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PurchaseOrderStatus
{
    public record PurchaseOrderStatusName
    {
        public string Value { get; }

        public PurchaseOrderStatusName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Purchase order status name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Purchase order status name cannot exceed 50 characters.");

            if (value is not ("Pending" or "Sent" or "Received" or "Cancelled"))
                throw new ArgumentException("Purchase order status must be Pending, Sent, Received or Cancelled.");

            Value = value.Trim();
        }
    }
}