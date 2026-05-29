namespace Application.DTOs.Parts;

public sealed class PartDto
{
    public int Id { get; init; }
    public int PartCategoryId { get; init; }
    public string PartCategoryName { get; init; } = string.Empty;
    public int? UnitId { get; init; }
    public string? UnitName { get; init; }
    public string? UnitAbbreviation { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Stock { get; init; }
    public int MinStock { get; init; }
    public decimal UnitPrice { get; init; }
    public bool IsActive { get; init; }
}
