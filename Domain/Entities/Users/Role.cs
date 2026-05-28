using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Users.Role;
namespace Domain.Entities.Users
{
    public sealed class Role : BaseEntity
    {
        public RoleName RoleName { get; private set; } = null!;

        public ICollection<UserRole> UserRoles { get; private set; } = [];

        private Role() { }

        public Role(RoleName roleName)
        {
            RoleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
        }

        public void Update(RoleName roleName)
        {
            RoleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
        }
    }
}