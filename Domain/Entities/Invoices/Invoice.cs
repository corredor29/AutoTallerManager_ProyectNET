using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Quotations;
using Domain.Entities.ServiceOrders;

namespace Domain.Entities.Invoices
{
    public class Invoice : BaseEntity
    {
        public int      ServiceOrderId       { get; set; }
        public int?     QuotationId          { get; set; }
        public DateTime IssuedAt             { get; set; } = DateTime.UtcNow;
        public decimal  LaborCost            { get; set; } = 0;
        public decimal  Subtotal             { get; set; } = 0;
        public decimal  Tax                  { get; set; } = 0;
        public decimal  Total                { get; set; } = 0;
        public bool     DiagnosisOnlyCharged { get; set; } = false;

        public ServiceOrder              ServiceOrder { get; set; } = null!;
        public Quotation?                Quotation    { get; set; }
        public ICollection<InvoiceDetail> Details     { get; set; } = default!;
        public ICollection<Payment>       Payments    { get; set; } = default!;
    }
}