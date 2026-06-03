using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Auth
{
    public sealed class RegisterRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; init; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; init; } = string.Empty;

        [StringLength(50)]
        public string? Role { get; init; }
    }
}