using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Appointments.AppointmentStatus;
namespace Domain.Entities.Appointments
{

    public sealed class AppointmentStatus : BaseEntity
    {
        public AppointmentStatusName Name { get; private set; } = null!;

        public ICollection<Appointment> Appointments { get; private set; } = [];

        private AppointmentStatus() { }

        public AppointmentStatus(AppointmentStatusName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(AppointmentStatusName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}