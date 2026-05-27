using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Persons;
using Domain.Entities.Vehicles;
using Domain.Entities.Appointments;

namespace Domain.Entities.Customers
{
    public class Customer : BaseEntity
    {
        public int  PersonId { get; set; }
        public bool IsActive { get; set; } = true;

        public Person   Person       { get; set; } = null!;
        public ICollection<VehicleOwnershipHistory> Ownerships   { get; set; } = default!;
        public ICollection<Appointment>    Appointments { get; set; } = default!;
    }
}