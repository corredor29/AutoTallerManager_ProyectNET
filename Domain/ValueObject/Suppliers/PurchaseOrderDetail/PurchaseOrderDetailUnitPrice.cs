using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PurchaseOrderDetail
{
    public record PurchaseOrderDetailUnitPrice
    {
        public decimal Value { get; }

        public PurchaseOrderDetailUnitPrice(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            Value = value;
        }
    }
}