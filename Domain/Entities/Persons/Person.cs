using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Customers;
using Domain.Entities.Users;

namespace Domain.Entities.Persons
{
    public class Person : BaseEntity
    {
        public string    FirstName    { get; set; } = null!;
        public string    LastName     { get; set; } = null!;
        public DateTime  RegisteredAt { get; set; } = DateTime.UtcNow;

        public Customer? Customer  { get; set; }
        public User?  User      { get; set; }
        public ICollection<PersonDocument> Documents { get; set; } = default!;
        public ICollection<PersonEmail>    Emails    { get; set; } = default!;
        public ICollection<PersonPhone>    Phones    { get; set; } = default!;
    }
}