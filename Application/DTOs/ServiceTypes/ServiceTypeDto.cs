namespace Application.DTOs.ServiceTypes;

public sealed class ServiceTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int? EstimatedDurationHours { get; init; }
}
