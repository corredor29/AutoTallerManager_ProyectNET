using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Users
{
    public sealed class UpdateUserRequest
    {
        [StringLength(255, MinimumLength = 8)]
        public string? Password { get; init; }

        public bool IsActive { get; init; }

        public IEnumerable<int> RoleIds { get; init; } = [];
    }
}