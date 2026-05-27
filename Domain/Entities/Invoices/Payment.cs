using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Invoices
{
    public class Payment : BaseEntity
    {
        public int      InvoiceId       { get; set; }
        public int      PaymentMethodId { get; set; }
        public decimal  Amount          { get; set; }
        public DateTime PaidAt          { get; set; } = DateTime.UtcNow;
        public string?  Reference       { get; set; }

        public Invoice       Invoice       { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; } = null!;
}
}