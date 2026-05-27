using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Customers;
using Domain.Entities.Users;
using Domain.Entities.Vehicles;
using Domain.Entities.ServiceOrders;

namespace Domain.Entities.Appointments
{
    public class Appointment : BaseEntity
    {
        public int       CustomerId          { get; set; }
        public int       VehicleId           { get; set; }
        public int       ServiceTypeId       { get; set; }
        public int       AppointmentStatusId { get; set; }
        public int?      AssignedUserId      { get; set; }
        public DateTime  AppointmentDate     { get; set; }
        public string?   Notes               { get; set; }

        public Customer          Customer          { get; set; } = null!;
        public Vehicle           Vehicle           { get; set; } = null!;
        public ServiceType       ServiceType       { get; set; } = null!;
        public AppointmentStatus AppointmentStatus { get; set; } = null!;
        public User?             AssignedUser      { get; set; }
        public ServiceOrder?     ServiceOrder      { get; set; }
    }

}