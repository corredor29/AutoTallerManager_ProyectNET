namespace Application.Requests.InvoiceDetails;

public sealed class UpdateInvoiceDetailRequest
{
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
