namespace Api.Responses;

public sealed class ApiSuccessResponse<T>
{
    public bool Success { get; init; } = true;
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public T Data { get; init; } = default!;
    public string? TraceId { get; init; }
}
