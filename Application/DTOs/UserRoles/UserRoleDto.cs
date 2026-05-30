using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.UserRoles
{
    public sealed class UserRoleDto
    {
        public int    UserId   { get; init; }
        public int    RoleId   { get; init; }
        public string RoleName { get; init; } = string.Empty;
    }
}