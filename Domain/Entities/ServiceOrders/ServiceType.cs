using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Appointments;
using Domain.ValueObject.ServiceOrders.ServiceType;
namespace Domain.Entities.ServiceOrders
{
    public sealed class ServiceType : BaseEntity
    {
        public ServiceTypeName Name              { get; private set; } = null!;
        public ServiceDuration EstimatedDuration { get; private set; } = null!;

        public ICollection<ServiceOrder> ServiceOrders { get; private set; } = [];
        public ICollection<Appointment>  Appointments  { get; private set; } = [];

        private ServiceType() { }

        public ServiceType(ServiceTypeName name, ServiceDuration estimatedDuration)
        {
            Name              = name              ?? throw new ArgumentNullException(nameof(name));
            EstimatedDuration = estimatedDuration ?? throw new ArgumentNullException(nameof(estimatedDuration));
        }

        public void Update(ServiceTypeName name, ServiceDuration estimatedDuration)
        {
            Name              = name              ?? throw new ArgumentNullException(nameof(name));
            EstimatedDuration = estimatedDuration ?? throw new ArgumentNullException(nameof(estimatedDuration));
        }
    }
}