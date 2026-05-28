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
using Domain.ValueObject.ServiceOrders.ServiceOrder;

namespace Domain.Entities.ServiceOrders
{
public sealed class ServiceOrder : BaseEntity
{
    public int                VehicleId           { get; private set; }
    public int                ServiceTypeId       { get; private set; }
    public int                MechanicId          { get; private set; }
    public int                OrderStatusId       { get; private set; }
    public int?               AppointmentId       { get; private set; }
    public DateTime           CreatedAt           { get; private set; }
    public DateTime?          EstimatedDeliveryAt { get; private set; }
    public DateTime?          ClosedAt            { get; private set; }
    public WorkDescription    WorkPerformed       { get; private set; } = null!;
    public ServiceOrderNotes  Notes               { get; private set; } = null!;

    public Vehicle                       Vehicle       { get; private set; } = null!;
    public ServiceType                   ServiceType   { get; private set; } = null!;
    public User                          Mechanic      { get; private set; } = null!;
    public OrderStatus                   OrderStatus   { get; private set; } = null!;
    public Appointment?                  Appointment   { get; private set; }
    public ICollection<ServiceOrderPart> Parts         { get; private set; } = [];
    public ICollection<Quotation>        Quotations    { get; private set; } = [];
    public Invoice?                      Invoice       { get; private set; }

    private ServiceOrder() { }

    public ServiceOrder(int vehicleId, int serviceTypeId, int mechanicId,
                        int orderStatusId, WorkDescription workPerformed,
                        ServiceOrderNotes notes, int? appointmentId = null,
                        DateTime? estimatedDeliveryAt = null)
    {
        VehicleId           = vehicleId     > 0 ? vehicleId     : throw new ArgumentException("VehicleId must be greater than 0.");
        ServiceTypeId       = serviceTypeId > 0 ? serviceTypeId : throw new ArgumentException("ServiceTypeId must be greater than 0.");
        MechanicId          = mechanicId    > 0 ? mechanicId    : throw new ArgumentException("MechanicId must be greater than 0.");
        OrderStatusId       = orderStatusId > 0 ? orderStatusId : throw new ArgumentException("OrderStatusId must be greater than 0.");
        WorkPerformed       = workPerformed ?? throw new ArgumentNullException(nameof(workPerformed));
        Notes               = notes         ?? throw new ArgumentNullException(nameof(notes));
        AppointmentId       = appointmentId;
        EstimatedDeliveryAt = estimatedDeliveryAt;
        CreatedAt           = DateTime.UtcNow;
    }

    public void Update(WorkDescription workPerformed, ServiceOrderNotes notes,
                       DateTime? estimatedDeliveryAt)
    {
        WorkPerformed       = workPerformed ?? throw new ArgumentNullException(nameof(workPerformed));
        Notes               = notes         ?? throw new ArgumentNullException(nameof(notes));
        EstimatedDeliveryAt = estimatedDeliveryAt;
    }

    public void ChangeStatus(int orderStatusId)
    {
        OrderStatusId = orderStatusId > 0 ? orderStatusId : throw new ArgumentException("OrderStatusId must be greater than 0.");
    }

    public void Close()
    {
        ClosedAt = DateTime.UtcNow;
    }
}
}