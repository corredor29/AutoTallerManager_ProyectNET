using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Vehicles
{
    public sealed class UpdateVehicleRequest
    {
        public int? ColorId { get; init; }
        public int? FuelTypeId { get; init; }
        public int? TransmissionTypeId { get; init; }

        [Range(1886, 2026)]
        public short Year { get; init; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; init; }

        [StringLength(20)]
        public string? LicensePlate { get; init; }
    }
}
