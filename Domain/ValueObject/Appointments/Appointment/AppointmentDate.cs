using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Appointments.Appointment
{
    public record AppointmentDate
    {
        public DateTime Value { get; }

        public AppointmentDate(DateTime value)
        {
            if (value == default)
                throw new ArgumentException("Appointment date cannot be empty.");

            if (value < DateTime.UtcNow)
                throw new ArgumentException("Appointment date cannot be in the past.");

            Value = value;
        }
    }
}