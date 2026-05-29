namespace Application.Requests.PartSuppliers;

public sealed class UpdatePartSupplierRequest
{
    public decimal PurchasePrice { get; init; }
    public bool IsPrimary { get; init; }
}
