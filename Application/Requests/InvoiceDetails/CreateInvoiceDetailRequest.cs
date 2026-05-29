namespace Application.Requests.InvoiceDetails;

public sealed class CreateInvoiceDetailRequest
{
    public int InvoiceId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
