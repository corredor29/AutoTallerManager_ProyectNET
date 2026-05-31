namespace Application.Filters;

public sealed class PartFilter
{
    public string? Description    { get; init; }
    public string? Code           { get; init; }
    public int?    PartCategoryId { get; init; }
    public bool?   IsActive       { get; init; }
    public bool?   BelowMinStock  { get; init; }
}
