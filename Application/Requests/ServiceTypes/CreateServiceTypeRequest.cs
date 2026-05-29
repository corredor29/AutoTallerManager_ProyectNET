namespace Application.Requests.ServiceTypes;

public sealed class CreateServiceTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public int EstimatedDurationHours { get; init; }
}
