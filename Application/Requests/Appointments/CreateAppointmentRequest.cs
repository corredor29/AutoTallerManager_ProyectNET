namespace Application.Requests.Appointments;

public sealed class CreateAppointmentRequest
{
    public int CustomerId { get; init; }
    public int VehicleId { get; init; }
    public int ServiceTypeId { get; init; }
    public int AppointmentStatusId { get; init; }
    public int? AssignedUserId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public string? Notes { get; init; }
}
