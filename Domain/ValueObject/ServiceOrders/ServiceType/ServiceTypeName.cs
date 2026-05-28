using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.ServiceOrders.ServiceType
{
    public record ServiceTypeName
    {
        public string Value { get; }

        public ServiceTypeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Service type name cannot be empty.");

            if (value.Length > 80)
                throw new ArgumentException("Service type name cannot exceed 80 characters.");

            Value = value.Trim();
        }
    }
}