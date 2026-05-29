using System.ComponentModel.DataAnnotations;

namespace Application.Requests.ServiceOrders;

public sealed class UpdateServiceOrderRequest
{
    public DateTime? EstimatedDeliveryAt { get; init; }

    [StringLength(2000)]
    public string? WorkPerformed { get; init; }

    [StringLength(500)]
    public string? Notes { get; init; }
}
