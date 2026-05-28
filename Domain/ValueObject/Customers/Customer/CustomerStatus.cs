using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Customers.Customer
{
    public record CustomerStatus
    {
        public bool Value { get; }

        public CustomerStatus(bool value)
        {
            Value = value;
        }

        public static CustomerStatus Active   => new(true);
        public static CustomerStatus Inactive => new(false);

        public bool IsActive   => Value == true;
        public bool IsInactive => Value == false;
    }
}