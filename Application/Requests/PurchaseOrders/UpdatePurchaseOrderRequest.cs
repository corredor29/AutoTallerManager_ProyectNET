namespace Application.Requests.PurchaseOrders;

public sealed class UpdatePurchaseOrderRequest
{
    public decimal Total { get; init; }
    public string? Notes { get; init; }
}
