using System.ComponentModel.DataAnnotations;

namespace Application.Requests.ServiceOrders;

public sealed class ChangeServiceOrderStatusRequest
{
    [Range(1, int.MaxValue)]
    public int OrderStatusId { get; init; }
}
