using Application.Common.Pagination;

namespace Application.Requests.Parts;

public sealed class GetPartsRequest : PagedQuery
{
    public string? Search { get; init; }
    public int? PartCategoryId { get; init; }
    public bool? IsActive { get; init; }
    public bool? LowStockOnly { get; init; }
}
