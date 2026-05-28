using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Customers;
using Domain.Entities.Users;
using Domain.ValueObject.Persons.Person;
namespace Domain.Entities.Persons
{
    public sealed class Person : BaseEntity
    {
        public PersonFirstName FirstName    { get; private set; } = null!;
        public PersonLastName  LastName     { get; private set; } = null!;
        public DateTime        RegisteredAt { get; private set; }

        public Customer?                   Customer  { get; private set; }
        public User?                       User      { get; private set; }
        public ICollection<PersonDocument> Documents { get; private set; } = [];
        public ICollection<PersonEmail>    Emails    { get; private set; } = [];
        public ICollection<PersonPhone>    Phones    { get; private set; } = [];

        private Person() { }

        public Person(PersonFirstName firstName, PersonLastName lastName)
        {
            FirstName    = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName     = lastName  ?? throw new ArgumentNullException(nameof(lastName));
            RegisteredAt = DateTime.UtcNow;
        }

        public void Update(PersonFirstName firstName, PersonLastName lastName)
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName  = lastName  ?? throw new ArgumentNullException(nameof(lastName));
        }
    }
}