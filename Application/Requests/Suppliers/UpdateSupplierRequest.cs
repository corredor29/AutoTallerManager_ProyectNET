namespace Application.Requests.Suppliers;

public sealed class UpdateSupplierRequest
{
    public string CompanyName { get; init; } = string.Empty;
    public string? TaxId { get; init; }
    public string? ContactName { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
}
