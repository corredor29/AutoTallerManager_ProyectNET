namespace Application.DTOs.Appointments;

public sealed class AppointmentDto
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int VehicleId { get; init; }
    public string VehicleVin { get; init; } = string.Empty;
    public string VehicleDisplayName { get; init; } = string.Empty;
    public int ServiceTypeId { get; init; }
    public string ServiceTypeName { get; init; } = string.Empty;
    public int AppointmentStatusId { get; init; }
    public string AppointmentStatusName { get; init; } = string.Empty;
    public int? AssignedUserId { get; init; }
    public string AssignedUserName { get; init; } = string.Empty;
    public DateTime AppointmentDate { get; init; }
    public string? Notes { get; init; }
}
