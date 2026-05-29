namespace Application.DTOs.Persons
{
    public sealed class PersonDto
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public DateTime RegisteredAt { get; init; }
    }
}
