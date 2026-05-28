using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PurchaseOrderDetail
{
    public record PurchaseOrderDetailQuantity
    {
        public int Value { get; }

        public PurchaseOrderDetailQuantity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than 0.");

            Value = value;
        }
    }
}