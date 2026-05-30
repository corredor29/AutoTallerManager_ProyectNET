using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Users
{
    public sealed class UserDto
    {
        public int    Id        { get; init; }
        public int    PersonId  { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName  { get; init; } = string.Empty;
        public bool   IsActive  { get; init; }
        public IEnumerable<string> Roles { get; init; } = [];
    }
}