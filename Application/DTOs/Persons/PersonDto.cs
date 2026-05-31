namespace Application.DTOs.Persons
{
    public sealed class PersonDto
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? PrimaryEmail { get; init; }
        public string? PrimaryPhone { get; init; }
        public DateTime RegisteredAt { get; init; }
    }
}
