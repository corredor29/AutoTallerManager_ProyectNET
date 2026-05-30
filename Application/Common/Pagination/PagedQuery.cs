namespace Application.Common.Pagination;

public abstract class PagedQuery
{
    private const int MaxPageSize = 100;

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public int NormalizedPageNumber => PageNumber < 1 ? 1 : PageNumber;

    public int NormalizedPageSize
    {
        get
        {
            if (PageSize < 1)
            {
                return 10;
            }

            return PageSize > MaxPageSize ? MaxPageSize : PageSize;
        }
    }
}
