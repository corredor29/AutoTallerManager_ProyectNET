using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Persons.PersonEmail;
namespace Domain.Entities.Persons
{
    public sealed class PersonEmail : BaseEntity
    {
        public int       PersonId      { get; private set; }
        public int       EmailDomainId { get; private set; }
        public EmailUser EmailUser     { get; private set; } = null!;
        public bool      IsPrimary     { get; private set; }

        public Person      Person      { get; private set; } = null!;
        public EmailDomain EmailDomain { get; private set; } = null!;

        private PersonEmail() { }

        public PersonEmail(int personId, int emailDomainId, EmailUser emailUser, bool isPrimary = false)
        {
            PersonId      = personId      > 0 ? personId      : throw new ArgumentException("PersonId must be greater than 0.");
            EmailDomainId = emailDomainId > 0 ? emailDomainId : throw new ArgumentException("EmailDomainId must be greater than 0.");
            EmailUser     = emailUser ?? throw new ArgumentNullException(nameof(emailUser));
            IsPrimary     = isPrimary;
        }

        public void Update(EmailUser emailUser, bool isPrimary)
        {
            EmailUser = emailUser ?? throw new ArgumentNullException(nameof(emailUser));
            IsPrimary = isPrimary;
        }

        public void SetAsPrimary()   => IsPrimary = true;
        public void SetAsSecondary() => IsPrimary = false;
    }
}