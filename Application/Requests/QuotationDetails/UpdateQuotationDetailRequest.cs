namespace Application.Requests.QuotationDetails;

public sealed class UpdateQuotationDetailRequest
{
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
