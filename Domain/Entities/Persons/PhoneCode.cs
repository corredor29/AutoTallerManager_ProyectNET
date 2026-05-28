using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Persons.PhoneCode;

namespace Domain.Entities.Persons
{

    public sealed class PhoneCode : BaseEntity
    {
        public PhoneCodeValue   Code    { get; private set; } = null!;
        public PhoneCodeCountry Country { get; private set; } = null!;

        public ICollection<PersonPhone> PersonPhones { get; private set; } = [];

        private PhoneCode() { }

        public PhoneCode(PhoneCodeValue code, PhoneCodeCountry country)
        {
            Code    = code    ?? throw new ArgumentNullException(nameof(code));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }

        public void Update(PhoneCodeValue code, PhoneCodeCountry country)
        {
            Code    = code    ?? throw new ArgumentNullException(nameof(code));
            Country = country ?? throw new ArgumentNullException(nameof(country));
        }
    }
}