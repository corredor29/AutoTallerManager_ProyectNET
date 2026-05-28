using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Persons.PersonPhone;
namespace Domain.Entities.Persons
{
    public sealed class PersonPhone : BaseEntity
    {
        public int         PersonId    { get; private set; }
        public int         PhoneCodeId { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; } = null!;
        public bool        IsPrimary   { get; private set; }

        public Person    Person    { get; private set; } = null!;
        public PhoneCode PhoneCode { get; private set; } = null!;

        private PersonPhone() { }

        public PersonPhone(int personId, int phoneCodeId, PhoneNumber phoneNumber, bool isPrimary = false)
        {
            PersonId    = personId    > 0 ? personId    : throw new ArgumentException("PersonId must be greater than 0.");
            PhoneCodeId = phoneCodeId > 0 ? phoneCodeId : throw new ArgumentException("PhoneCodeId must be greater than 0.");
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            IsPrimary   = isPrimary;
        }

        public void Update(PhoneNumber phoneNumber, bool isPrimary)
        {
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            IsPrimary   = isPrimary;
        }

        public void SetAsPrimary()   => IsPrimary = true;
        public void SetAsSecondary() => IsPrimary = false;
    }
}