using System;

namespace Domain.ValueObject.Appointments.Appointment
{
    public record AppointmentDate
    {
        public DateTime Value { get; }

        public AppointmentDate(DateTime value)
        {
            if (value == default)
                throw new ArgumentException("Appointment date cannot be empty.");

            Value = value;
        }
    }
}