using Application.Common.Pagination;

namespace Application.Requests.Invoices;

public sealed class GetInvoicesRequest : PagedQuery
{
    public int? ServiceOrderId { get; init; }
    public int? QuotationId { get; init; }
    public DateTime? IssuedFrom { get; init; }
    public DateTime? IssuedTo { get; init; }
    public decimal? MinTotal { get; init; }
    public decimal? MaxTotal { get; init; }
}
