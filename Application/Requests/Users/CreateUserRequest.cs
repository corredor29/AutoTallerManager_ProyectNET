using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Users
{
    public sealed class CreateUserRequest
    {
        [Required]
        public int PersonId { get; init; }

        [Required]
        [StringLength(255, MinimumLength = 8)]
        public string Password { get; init; } = string.Empty;

        [Required]
        public IEnumerable<int> RoleIds { get; init; } = [];
    }
}