using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Roles
{
    public sealed class RoleDto
    {
        public int    Id       { get; init; }
        public string RoleName { get; init; } = string.Empty;
    }
}