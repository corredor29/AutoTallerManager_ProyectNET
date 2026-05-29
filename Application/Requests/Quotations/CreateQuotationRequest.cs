namespace Application.Requests.Quotations;

public sealed class CreateQuotationRequest
{
    public int ServiceOrderId { get; init; }
    public int CreatedByUserId { get; init; }
    public int QuotationStatusId { get; init; }
    public decimal LaborCost { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Total { get; init; }
    public string? Notes { get; init; }
}
