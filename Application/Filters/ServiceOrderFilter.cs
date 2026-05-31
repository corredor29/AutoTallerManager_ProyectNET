namespace Application.Filters;

public sealed class ServiceOrderFilter
{
    public int?      OrderStatusId { get; init; }
    public int?      MechanicId    { get; init; }
    public int?      ServiceTypeId { get; init; }
    public DateTime? DateFrom      { get; init; }
    public DateTime? DateTo        { get; init; }
}
