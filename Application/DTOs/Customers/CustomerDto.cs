namespace Application.DTOs.Customers
{
    public sealed class CustomerDto
    {
        public int Id { get; init; }
        public int PersonId { get; init; }
        public bool IsActive { get; init; }
        public Application.DTOs.Persons.PersonDto Person { get; init; } = null!;
    }
}
