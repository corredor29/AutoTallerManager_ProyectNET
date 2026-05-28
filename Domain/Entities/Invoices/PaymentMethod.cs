using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Invoices.PaymentMethod;

namespace Domain.Entities.Invoices
{
    public sealed class PaymentMethod : BaseEntity
    {
        public PaymentMethodName Name { get; private set; } = null!;

        public ICollection<Payment> Payments { get; private set; } = [];

        private PaymentMethod() { }

        public PaymentMethod(PaymentMethodName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(PaymentMethodName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}