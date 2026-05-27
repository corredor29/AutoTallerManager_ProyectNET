using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Appointments
{
    public class AppointmentStatus : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = default!;
    }
}