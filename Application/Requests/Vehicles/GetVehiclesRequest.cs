using Application.Common.Pagination;

namespace Application.Requests.Vehicles;

public sealed class GetVehiclesRequest : PagedQuery
{
    public string? Search       { get; init; }
    public string? Vin          { get; init; }
    public string? Brand        { get; init; }
    public string? Model        { get; init; }
    public string? LicensePlate { get; init; }
    public int?    Year         { get; init; }
}
