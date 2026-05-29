namespace Application.Requests.Suppliers;

public sealed class CreateSupplierRequest
{
    public string CompanyName { get; init; } = string.Empty;
    public string? TaxId { get; init; }
    public string? ContactName { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
}
