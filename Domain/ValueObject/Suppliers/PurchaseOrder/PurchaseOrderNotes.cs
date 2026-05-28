using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PurchaseOrder
{
    public record PurchaseOrderNotes
    {
        public string? Value { get; }

        public PurchaseOrderNotes(string? value)
        {
            if (value is not null && value.Length > 500)
                throw new ArgumentException("Purchase order notes cannot exceed 500 characters.");

            Value = value?.Trim();
        }
    }
}