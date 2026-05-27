using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Appointments;
using Domain.Entities.Users;
using Domain.Entities.Vehicles;
using Domain.Entities.Parts;
using Domain.Entities.Quotations;
using Domain.Entities.Invoices;

namespace Domain.Entities.ServiceOrders
{
    public class ServiceOrder : BaseEntity
    {
        public int       VehicleId           { get; set; }
        public int       ServiceTypeId       { get; set; }
        public int       MechanicId          { get; set; }
        public int       OrderStatusId       { get; set; }
        public int?      AppointmentId       { get; set; }
        public DateTime  CreatedAt           { get; set; } = DateTime.UtcNow;
        public DateTime? EstimatedDeliveryAt { get; set; }
        public DateTime? ClosedAt            { get; set; }
        public string?   WorkPerformed       { get; set; }
        public string?   Notes               { get; set; }

        public Vehicle             Vehicle          { get; set; } = null!;
        public ServiceType         ServiceType      { get; set; } = null!;
        public User                Mechanic         { get; set; } = null!;
        public OrderStatus         OrderStatus      { get; set; } = null!;
        public Appointment?        Appointment      { get; set; }
        public ICollection<ServiceOrderPart> Parts  { get; set; } = default!;
        public ICollection<Quotation>    Quotations { get; set; } = default!;
        public Invoice?            Invoice          { get; set; }
    }
}