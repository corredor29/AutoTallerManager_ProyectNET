using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Persons.EmailDomain;
namespace Domain.Entities.Persons
{
    public sealed class EmailDomain : BaseEntity
    {
        public EmailDomainValue Domain { get; private set; } = null!;

        public ICollection<PersonEmail> PersonEmails { get; private set; } = [];

        private EmailDomain() { }

        public EmailDomain(EmailDomainValue domain)
        {
            Domain = domain ?? throw new ArgumentNullException(nameof(domain));
        }

        public void Update(EmailDomainValue domain)
        {
            Domain = domain ?? throw new ArgumentNullException(nameof(domain));
        }
    }
}