using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Customers
{
    public sealed class CreateCustomerRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; init; } = string.Empty;

        [EmailAddress]
        public string? Email { get; init; }

        [StringLength(20)]
        public string? Phone { get; init; }

        public int? DocumentTypeId { get; init; }

        [StringLength(50)]
        public string? DocumentNumber { get; init; }
    }
}