using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Appointments;

namespace Domain.Entities.ServiceOrders
{
    public class ServiceType : BaseEntity
    {
        public string Name              { get; set; } = null!;
        public int?   EstimatedDuration { get; set; }

        public ICollection<ServiceOrder>  ServiceOrders { get; set; } = [];
        public ICollection<Appointment>   Appointments  { get; set; } = [];
    }
}