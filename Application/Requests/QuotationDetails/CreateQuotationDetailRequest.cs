namespace Application.Requests.QuotationDetails;

public sealed class CreateQuotationDetailRequest
{
    public int QuotationId { get; init; }
    public int PartId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
