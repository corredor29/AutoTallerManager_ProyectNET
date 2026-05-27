using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Persons;

namespace Domain.Entities.Users
{
    public class User : BaseEntity
    {
        public int    PersonId     { get; set; }
        public string PasswordHash { get; set; } = null!;
        public bool   IsActive     { get; set; } = true;

        public Person                Person    { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}