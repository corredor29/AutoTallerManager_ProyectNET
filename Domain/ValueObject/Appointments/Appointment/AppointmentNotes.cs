using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Appointments.Appointment
{

    public record AppointmentNotes
    {
        public string? Value { get; }

        public AppointmentNotes(string? value)
        {
            if (value is not null && value.Length > 500)
                throw new ArgumentException("Appointment notes cannot exceed 500 characters.");

            Value = value?.Trim();
        }
    }
}