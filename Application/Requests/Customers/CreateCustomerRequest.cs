namespace Application.Requests.Customers
{
    public sealed class CreateCustomerRequest
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }
}
