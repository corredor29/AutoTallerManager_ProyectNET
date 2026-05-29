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
    }
}
