namespace Application.Requests.MeasurementUnits;

public sealed class CreateMeasurementUnitRequest
{
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
}
