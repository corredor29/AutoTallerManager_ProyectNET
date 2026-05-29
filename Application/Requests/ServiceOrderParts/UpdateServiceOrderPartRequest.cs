namespace Application.Requests.ServiceOrderParts;

public sealed class UpdateServiceOrderPartRequest
{
    public int Quantity { get; init; }
    public decimal AppliedUnitPrice { get; init; }
}
