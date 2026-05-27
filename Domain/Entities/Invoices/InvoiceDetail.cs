using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Invoices
{
    public class InvoiceDetail : BaseEntity
    {
        public int     InvoiceId   { get; set; }
        public string  Description { get; set; } = null!;
        public int     Quantity    { get; set; } = 1;
        public decimal UnitPrice   { get; set; }

        public Invoice Invoice { get; set; } = null!;
    }
}