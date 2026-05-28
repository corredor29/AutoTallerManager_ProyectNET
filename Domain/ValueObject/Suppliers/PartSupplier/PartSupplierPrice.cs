using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PartSupplier
{
    public record PartSupplierPrice
    {
        public decimal Value { get; }

        public PartSupplierPrice(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Purchase price cannot be negative.");

            Value = value;
        }
    }
}