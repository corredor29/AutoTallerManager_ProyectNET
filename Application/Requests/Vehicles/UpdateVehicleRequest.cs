namespace Application.Requests.Vehicles
{
    public sealed class UpdateVehicleRequest
    {
        public int? ColorId { get; init; }
        public int? FuelTypeId { get; init; }
        public int? TransmissionTypeId { get; init; }
        public short Year { get; init; }
        public int Mileage { get; init; }
        public string? LicensePlate { get; init; }
    }
}
