using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Persons;
using Domain.ValueObject.Users.User;
namespace Domain.Entities.Users
{
    public sealed class User : BaseEntity
    {
        public int          PersonId     { get; private set; }
        public PasswordHash PasswordHash { get; private set; } = null!;
        public bool         IsActive     { get; private set; }

        public Person                Person    { get; private set; } = null!;
        public ICollection<UserRole> UserRoles { get; private set; } = [];

        private User() { }

        public User(int personId, PasswordHash passwordHash)
        {
            PersonId     = personId > 0 ? personId : throw new ArgumentException("PersonId must be greater than 0.");
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            IsActive     = true;
        }

        public void UpdatePassword(PasswordHash passwordHash)
        {
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

        public void Activate()   => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}