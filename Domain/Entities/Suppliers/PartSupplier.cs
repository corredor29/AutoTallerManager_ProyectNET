using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Parts;

namespace Domain.Entities.Suppliers
{
    public class PartSupplier : BaseEntity
    {
        public int     PartId        { get; set; }
        public int     SupplierId    { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool    IsPrimary     { get; set; } = false;

        public Part     Part     { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
    }
}