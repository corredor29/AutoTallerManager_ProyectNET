using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Customers
{
    public sealed class UpdateCustomerRequest
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; init; } = string.Empty;

        public bool IsActive { get; init; }
    }
}
