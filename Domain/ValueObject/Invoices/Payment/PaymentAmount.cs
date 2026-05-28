using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Invoices.Payment
{
    public record PaymentAmount
    {
        public decimal Value { get; }

        public PaymentAmount(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Payment amount must be greater than 0.");

            Value = value;
        }
    }
}