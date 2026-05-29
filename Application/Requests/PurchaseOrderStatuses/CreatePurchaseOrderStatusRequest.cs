namespace Application.Requests.PurchaseOrderStatuses;

public sealed class CreatePurchaseOrderStatusRequest
{
    public string Name { get; init; } = string.Empty;
}
