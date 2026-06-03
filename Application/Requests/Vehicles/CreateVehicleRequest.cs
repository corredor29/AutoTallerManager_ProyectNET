using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Vehicles
{
    public sealed class CreateVehicleRequest
    {
        [Range(1, int.MaxValue)]
        public int ModelId { get; init; }
        public int? ColorId { get; init; }
        public int? FuelTypeId { get; init; }
        public int? TransmissionTypeId { get; init; }

        [Required]
        [StringLength(17, MinimumLength = 17)]
        public string Vin { get; init; } = string.Empty;

        [Range(1886, 2026)]
        public short Year { get; init; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; init; }

        [StringLength(20)]
        public string? LicensePlate { get; init; }
        public int? CustomerId { get; init; }
    }
}
