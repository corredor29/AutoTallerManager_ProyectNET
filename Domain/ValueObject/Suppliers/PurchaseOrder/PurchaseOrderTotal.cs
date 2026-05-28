using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PurchaseOrder
{
    public record PurchaseOrderTotal
    {
        public decimal Value { get; }

        public PurchaseOrderTotal(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Purchase order total cannot be negative.");

            Value = value;
        }
    }
}