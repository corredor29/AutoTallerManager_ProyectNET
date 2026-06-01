namespace Application.DTOs.Customers
{
    public sealed class CustomerDto
    {
        public int    Id             { get; init; }
        public int    PersonId       { get; init; }
        public bool   IsActive       { get; init; }
        public string FirstName      { get; init; } = string.Empty;
        public string LastName       { get; init; } = string.Empty;
        public string PrimaryEmail   { get; init; } = string.Empty;
        public string PrimaryPhone   { get; init; } = string.Empty;
        public string PrimaryDocument { get; init; } = string.Empty;
        public Persons.PersonDto Person { get; init; } = null!;
    }
}