using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Customers;

public sealed class RegisterCustomerWithVehicleRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int PhoneCodeId { get; init; }

    [Required]
    [StringLength(30)]
    public string PhoneNumber { get; init; } = string.Empty;

    [Required]
    public RegisterVehicleRequest Vehicle { get; init; } = new();
}

public sealed class RegisterVehicleRequest
{
    [Range(1, int.MaxValue)]
    public int ModelId { get; init; }

    public int? ColorId { get; init; }
    public int? FuelTypeId { get; init; }
    public int? TransmissionTypeId { get; init; }

    [Required]
    [StringLength(17, MinimumLength = 17)]
    public string Vin { get; init; } = string.Empty;

    [Range(1886, short.MaxValue)]
    public short Year { get; init; }

    [Range(0, int.MaxValue)]
    public int Mileage { get; init; }

    [StringLength(20)]
    public string? LicensePlate { get; init; }
}
