namespace Application.Requests.ServiceOrderParts;

public sealed class CreateServiceOrderPartRequest
{
    public int ServiceOrderId { get; init; }
    public int PartId { get; init; }
    public int Quantity { get; init; }
    public decimal AppliedUnitPrice { get; init; }
}
