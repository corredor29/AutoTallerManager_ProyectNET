namespace Application.Requests.Vehicles
{
    public sealed class CreateVehicleRequest
    {
        public int ModelId { get; init; }
        public int? ColorId { get; init; }
        public int? FuelTypeId { get; init; }
        public int? TransmissionTypeId { get; init; }
        public string Vin { get; init; } = string.Empty;
        public short Year { get; init; }
        public int Mileage { get; init; }
        public string? LicensePlate { get; init; }
    }
}
