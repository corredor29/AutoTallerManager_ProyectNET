using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Quotations
{
    public class QuotationStatus : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Quotation> Quotations { get; set; } = default!;
    }
}