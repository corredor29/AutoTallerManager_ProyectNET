using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.UserRoles
{
    public sealed class AssignRoleRequest
    {
        [Required]
        public int UserId { get; init; }

        [Required]
        public int RoleId { get; init; }
    }
}