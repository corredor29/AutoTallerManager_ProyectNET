using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Suppliers
{
    public class Supplier : BaseEntity
    {
        public string  CompanyName { get; set; } = null!;
        public string? TaxId       { get; set; }
        public string? ContactName { get; set; }
        public string? Phone       { get; set; }
        public string? Email       { get; set; }
        public string? Address     { get; set; }
        public bool    IsActive    { get; set; } = true;

        public ICollection<PartSupplier>    PartSuppliers  { get; set; } = default!;
        public ICollection<PurchaseOrder>   PurchaseOrders { get; set; } = default!;
    }
}