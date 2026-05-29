namespace Application.DTOs.MeasurementUnits;

public sealed class MeasurementUnitDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
}
