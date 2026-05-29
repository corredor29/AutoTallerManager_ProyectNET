namespace Application.DTOs.PurchaseOrderDetails;

public sealed class PurchaseOrderDetailDto
{
    public int Id { get; init; }
    public int PurchaseOrderId { get; init; }
    public int PartId { get; init; }
    public string PartCode { get; init; } = string.Empty;
    public string PartDescription { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
