namespace Application.DTOs.PartSuppliers;

public sealed class PartSupplierDto
{
    public int Id { get; init; }
    public int PartId { get; init; }
    public string PartCode { get; init; } = string.Empty;
    public string PartDescription { get; init; } = string.Empty;
    public int SupplierId { get; init; }
    public string SupplierCompanyName { get; init; } = string.Empty;
    public decimal PurchasePrice { get; init; }
    public bool IsPrimary { get; init; }
}
