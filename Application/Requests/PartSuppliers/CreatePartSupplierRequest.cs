namespace Application.Requests.PartSuppliers;

public sealed class CreatePartSupplierRequest
{
    public int PartId { get; init; }
    public int SupplierId { get; init; }
    public decimal PurchasePrice { get; init; }
    public bool IsPrimary { get; init; }
}
