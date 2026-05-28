using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Invoices.Payment;
namespace Domain.Entities.Invoices
{

    public sealed class Payment : BaseEntity
    {
        public int              InvoiceId       { get; private set; }
        public int              PaymentMethodId { get; private set; }
        public PaymentAmount    Amount          { get; private set; } = null!;
        public DateTime         PaidAt          { get; private set; }
        public PaymentReference Reference       { get; private set; } = null!;

        public Invoice       Invoice       { get; private set; } = null!;
        public PaymentMethod PaymentMethod { get; private set; } = null!;

        private Payment() { }

        public Payment(int invoiceId, int paymentMethodId,
                    PaymentAmount amount, PaymentReference reference)
        {
            InvoiceId       = invoiceId       > 0 ? invoiceId       : throw new ArgumentException("InvoiceId must be greater than 0.");
            PaymentMethodId = paymentMethodId > 0 ? paymentMethodId : throw new ArgumentException("PaymentMethodId must be greater than 0.");
            Amount          = amount    ?? throw new ArgumentNullException(nameof(amount));
            Reference       = reference ?? throw new ArgumentNullException(nameof(reference));
            PaidAt          = DateTime.UtcNow;
        }

        public void Update(PaymentAmount amount, PaymentReference reference)
        {
            Amount    = amount    ?? throw new ArgumentNullException(nameof(amount));
            Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        }
    }
}