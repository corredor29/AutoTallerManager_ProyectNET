using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Suppliers.PartSupplier
{
    public record PartSupplierIsPrimary
    {
        public bool Value { get; }

        public PartSupplierIsPrimary(bool value)
        {
            Value = value;
        }

        public static PartSupplierIsPrimary Primary   => new(true);
        public static PartSupplierIsPrimary Secondary => new(false);
    }
}