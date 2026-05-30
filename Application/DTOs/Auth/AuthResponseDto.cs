namespace Application.DTOs.Auth;

public sealed class AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public int UserId { get; init; }
    public int PersonId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = [];
}
