using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Users
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = null!;

        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}