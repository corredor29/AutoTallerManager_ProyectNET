namespace Application.Requests.PurchaseOrderDetails;

public sealed class CreatePurchaseOrderDetailRequest
{
    public int PurchaseOrderId { get; init; }
    public int PartId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
