namespace Application.Filters;

public sealed class VehicleFilter
{
    public string? VIN          { get; init; }
    public string? LicensePlate { get; init; }
    public int?    BrandId      { get; init; }
    public int?    ModelId      { get; init; }
    public short?  Year         { get; init; }
}
