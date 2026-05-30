using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.Roles
{
    public sealed class CreateRoleRequest
    {
        [Required]
        [StringLength(50)]
        public string RoleName { get; init; } = string.Empty;
    }
}