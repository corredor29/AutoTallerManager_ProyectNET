using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Auth;

public sealed class LoginRequest
{
    [Range(1, int.MaxValue)]
    public int UserId { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
