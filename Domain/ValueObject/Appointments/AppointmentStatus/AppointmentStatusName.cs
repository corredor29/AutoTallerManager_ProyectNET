using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Appointments.AppointmentStatus
{
    public record AppointmentStatusName
    {
        public string Value { get; }

        public AppointmentStatusName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Appointment status name cannot be empty.");

            if (value.Length > 50)
                throw new ArgumentException("Appointment status name cannot exceed 50 characters.");

            Value = value.Trim();
        }
    }
}