using Application.Common.Pagination;

namespace Application.Requests.ServiceOrders;

public sealed class GetServiceOrdersRequest : PagedQuery
{
    public int? VehicleId { get; init; }
    public int? MechanicId { get; init; }
    public int? OrderStatusId { get; init; }
    public string? Vin { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
}
