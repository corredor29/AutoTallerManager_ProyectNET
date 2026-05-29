namespace Application.Requests.Appointments;

public sealed class UpdateAppointmentRequest
{
    public int? AssignedUserId { get; init; }
    public DateTime AppointmentDate { get; init; }
    public string? Notes { get; init; }
}
