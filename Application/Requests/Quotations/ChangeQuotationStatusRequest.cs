namespace Application.Requests.Quotations;

public sealed class ChangeQuotationStatusRequest
{
    public int QuotationStatusId { get; init; }
    public string? RejectionReason { get; init; }
}
