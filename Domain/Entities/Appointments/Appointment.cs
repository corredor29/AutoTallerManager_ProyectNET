using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Customers;
using Domain.Entities.Users;
using Domain.Entities.Vehicles;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.Appointments.Appointment;
namespace Domain.Entities.Appointments
{
    public sealed class Appointment : BaseEntity
    {
        public int             CustomerId          { get; private set; }
        public int             VehicleId           { get; private set; }
        public int             ServiceTypeId       { get; private set; }
        public int             AppointmentStatusId { get; private set; }
        public int?            AssignedUserId      { get; private set; }
        public AppointmentDate AppointmentDate     { get; private set; } = null!;
        public AppointmentNotes Notes              { get; private set; } = null!;

        public Customer          Customer          { get; private set; } = null!;
        public Vehicle           Vehicle           { get; private set; } = null!;
        public ServiceType       ServiceType       { get; private set; } = null!;
        public AppointmentStatus AppointmentStatus { get; private set; } = null!;
        public User?             AssignedUser      { get; private set; }
        public ServiceOrder?     ServiceOrder      { get; private set; }

        private Appointment() { }

        public Appointment(int customerId, int vehicleId, int serviceTypeId,
                        int appointmentStatusId, AppointmentDate appointmentDate,
                        AppointmentNotes notes, int? assignedUserId = null)
        {
            CustomerId          = customerId          > 0 ? customerId          : throw new ArgumentException("CustomerId must be greater than 0.");
            VehicleId           = vehicleId           > 0 ? vehicleId           : throw new ArgumentException("VehicleId must be greater than 0.");
            ServiceTypeId       = serviceTypeId       > 0 ? serviceTypeId       : throw new ArgumentException("ServiceTypeId must be greater than 0.");
            AppointmentStatusId = appointmentStatusId > 0 ? appointmentStatusId : throw new ArgumentException("AppointmentStatusId must be greater than 0.");
            AppointmentDate     = appointmentDate ?? throw new ArgumentNullException(nameof(appointmentDate));
            Notes               = notes           ?? throw new ArgumentNullException(nameof(notes));
            AssignedUserId      = assignedUserId;
        }

        public void Update(AppointmentDate appointmentDate, AppointmentNotes notes, int? assignedUserId)
        {
            AppointmentDate = appointmentDate ?? throw new ArgumentNullException(nameof(appointmentDate));
            Notes           = notes           ?? throw new ArgumentNullException(nameof(notes));
            AssignedUserId  = assignedUserId;
        }

        public void ChangeStatus(int appointmentStatusId)
        {
            AppointmentStatusId = appointmentStatusId > 0 ? appointmentStatusId : throw new ArgumentException("AppointmentStatusId must be greater than 0.");
        }
    }
}