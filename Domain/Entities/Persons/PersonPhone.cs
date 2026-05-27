using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Persons
{
    public class PersonPhone : BaseEntity
    {
        public int    PersonId    { get; set; }
        public int    PhoneCodeId { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool   IsPrimary   { get; set; } = false;

        public Person    Person    { get; set; } = null!;
        public PhoneCode PhoneCode { get; set; } = null!;
    }
}