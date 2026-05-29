namespace Application.Requests.Parts;

public sealed class UpdatePartRequest
{
    public int PartCategoryId { get; init; }
    public int? UnitId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Stock { get; init; }
    public int MinStock { get; init; }
    public decimal UnitPrice { get; init; }
    public bool IsActive { get; init; }
}
