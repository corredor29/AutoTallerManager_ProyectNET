using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Quotations;
using Domain.Entities.Suppliers;

namespace Domain.Entities.Parts
{
    public class Part : BaseEntity
    {
        public int     PartCategoryId { get; set; }
        public int?    UnitId         { get; set; }
        public string  Code           { get; set; } = null!;
        public string  Description    { get; set; } = null!;
        public int     Stock          { get; set; } = 0;
        public int     MinStock       { get; set; } = 0;
        public decimal UnitPrice      { get; set; }
        public bool    IsActive       { get; set; } = true;

        public PartCategory                  Category      { get; set; } = null!;
        public MeasurementUnit?              Unit          { get; set; }
        public ICollection<ServiceOrderPart> OrderParts    { get; set; } = default!;
        public ICollection<PartSupplier>     PartSuppliers { get; set; } = default!;
        public ICollection<QuotationDetail>  QuotationDetails { get; set; } = default!;
    }
}