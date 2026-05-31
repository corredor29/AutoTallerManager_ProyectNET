namespace Application.DTOs.ServiceOrders;

public sealed class ServiceOrderDto
{
    public int Id { get; init; }
    public int? CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int VehicleId { get; init; }
    public string VehicleVin { get; init; } = string.Empty;
    public string VehicleDisplayName { get; init; } = string.Empty;
    public int ServiceTypeId { get; init; }
    public string ServiceTypeName { get; init; } = string.Empty;
    public int MechanicId { get; init; }
    public string MechanicName { get; init; } = string.Empty;
    public int OrderStatusId { get; init; }
    public string OrderStatusName { get; init; } = string.Empty;
    public int? AppointmentId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? EstimatedDeliveryAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public string? WorkPerformed { get; init; }
    public string? Notes { get; init; }
}
