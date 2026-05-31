using Application.DTOs.Vehicles;

namespace Application.DTOs.Customers;

public sealed class CustomerRegistrationDto
{
    public CustomerDto Customer { get; init; } = null!;
    public VehicleDto Vehicle { get; init; } = null!;
}
