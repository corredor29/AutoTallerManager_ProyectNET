using Application.Common.Pagination;

namespace Application.Requests.Customers;

public sealed class GetCustomersRequest : PagedQuery
{
    public string? Search         { get; init; }
    public string? DocumentNumber { get; init; }
    public bool?   IsActive       { get; init; }
}
