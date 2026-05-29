namespace Application.DTOs.PurchaseOrders;

public sealed class PurchaseOrderDto
{
    public int Id { get; init; }
    public int SupplierId { get; init; }
    public string SupplierCompanyName { get; init; } = string.Empty;
    public int UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public int PurchaseOrderStatusId { get; init; }
    public string PurchaseOrderStatusName { get; init; } = string.Empty;
    public DateTime OrderedAt { get; init; }
    public DateTime? ReceivedAt { get; init; }
    public decimal Total { get; init; }
    public string? Notes { get; init; }
}
