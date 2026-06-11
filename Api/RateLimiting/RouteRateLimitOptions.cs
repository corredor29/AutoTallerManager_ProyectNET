using Humanizer;

namespace Api.RateLimiting;

public sealed class RouteRateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public RouteLimit ServiceOrders { get; init; } = new();
    public RouteLimit Parts { get; init; } = new();
    public RouteLimit Admin {get; init;} =new();
    public RouteLimit Receptionist {get; init;} =new();
}

public sealed class RouteLimit
{
    public int PermitLimit { get; init; } = 60;
    public int WindowMinutes { get; init; } = 1;
    public int QueueLimit { get; init; } = 0;
}
