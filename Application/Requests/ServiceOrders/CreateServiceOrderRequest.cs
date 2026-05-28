using System.ComponentModel.DataAnnotations;

namespace Application.Requests.ServiceOrders;

public sealed class CreateServiceOrderRequest
{
    [Range(1, int.MaxValue)]
    public int VehicleId { get; init; }

    [Range(1, int.MaxValue)]
    public int ServiceTypeId { get; init; }

    [Range(1, int.MaxValue)]
    public int MechanicId { get; init; }

    [Range(1, int.MaxValue)]
    public int OrderStatusId { get; init; }

    public int? AppointmentId { get; init; }

    public DateTime? EstimatedDeliveryAt { get; init; }

    [StringLength(2000)]
    public string? WorkPerformed { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }
}
