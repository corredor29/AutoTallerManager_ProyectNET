using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Persons
{
    public class PersonEmail : BaseEntity
    {
        public int    PersonId      { get; set; }
        public int    EmailDomainId { get; set; }
        public string EmailUser     { get; set; } = null!;
        public bool   IsPrimary     { get; set; } = false;
        public Person      Person      { get; set; } = null!;
        public EmailDomain EmailDomain { get; set; } = null!;
    }
}