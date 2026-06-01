namespace Application.DTOs.Vehicles
{
    public sealed class VehicleDto
    {
        public int     Id                   { get; init; }
        public int     ModelId              { get; init; }
        public int?    ColorId              { get; init; }
        public int?    FuelTypeId           { get; init; }
        public int?    TransmissionTypeId   { get; init; }
        public string  Vin                  { get; init; } = string.Empty;
        public short   Year                 { get; init; }
        public int     Mileage              { get; init; }
        public string? LicensePlate         { get; init; }
        public string  ModelName            { get; init; } = string.Empty;
        public string  BrandName            { get; init; } = string.Empty;
        public string? ColorName            { get; init; }
        public string? FuelTypeName         { get; init; }
        public string? TransmissionTypeName { get; init; }
        public string? CurrentOwnerName     { get; init; } 
    }
}