using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Auth;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
