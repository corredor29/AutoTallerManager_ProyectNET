namespace Application.Requests.Customers
{
    public sealed class UpdateCustomerRequest
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
