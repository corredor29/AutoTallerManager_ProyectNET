namespace Application.Requests.PurchaseOrderDetails;

public sealed class UpdatePurchaseOrderDetailRequest
{
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
