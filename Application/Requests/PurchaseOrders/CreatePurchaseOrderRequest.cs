namespace Application.Requests.PurchaseOrders;

public sealed class CreatePurchaseOrderRequest
{
    public int SupplierId { get; init; }
    public int UserId { get; init; }
    public int PurchaseOrderStatusId { get; init; }
    public decimal Total { get; init; }
    public string? Notes { get; init; }
}
