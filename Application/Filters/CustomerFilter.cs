namespace Application.Filters;

public sealed class CustomerFilter
{
    public string? FirstName      { get; init; }
    public string? LastName       { get; init; }
    public string? DocumentNumber { get; init; }
    public bool?   IsActive       { get; init; }
}
