using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities.Users
{
    public sealed class UserRole
    {
        public int UserId { get; private set; }
        public int RoleId { get; private set; }

        public User User { get; private set; } = null!;
        public Role Role { get; private set; } = null!;

        private UserRole() { }

        public UserRole(int userId, int roleId)
        {
            UserId = userId > 0 ? userId : throw new ArgumentException("UserId must be greater than 0.");
            RoleId = roleId > 0 ? roleId : throw new ArgumentException("RoleId must be greater than 0.");
        }
    }
}