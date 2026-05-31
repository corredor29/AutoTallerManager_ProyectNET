namespace Application.Common.Pagination;

public sealed class PaginationParams
{
    private const int MaxPageSize = 50;

    public int PageNumber { get; init; } = 1;
    public int PageSize   { get; init; } = 10;

    public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;

    public int NormalizedPageSize =>
        PageSize < 1 ? 10 : PageSize > MaxPageSize ? MaxPageSize : PageSize;
}
