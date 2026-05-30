namespace Api.Responses;

public sealed class ApiErrorResponse
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
    public string? TraceId { get; init; }
    public IDictionary<string, string[]>? Errors { get; init; }
}
