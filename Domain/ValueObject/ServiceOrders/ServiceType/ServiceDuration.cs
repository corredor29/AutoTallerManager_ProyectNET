using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.ServiceOrders.ServiceType
{
    public record ServiceDuration
    {
        public int? Value { get; }

        public ServiceDuration(int? value)
        {
            if (value.HasValue && value.Value <= 0)
                throw new ArgumentException("Service duration must be greater than 0.");

            if (value.HasValue && value.Value > 720)
                throw new ArgumentException("Service duration cannot exceed 720 hours.");

            Value = value;
        }
    }
}