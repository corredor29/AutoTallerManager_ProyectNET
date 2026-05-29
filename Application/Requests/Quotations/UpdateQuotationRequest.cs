namespace Application.Requests.Quotations;

public sealed class UpdateQuotationRequest
{
    public decimal LaborCost { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Total { get; init; }
    public string? Notes { get; init; }
}
