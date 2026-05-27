using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Persons
{
    public class PhoneCode : BaseEntity
    {
        public string Code    { get; set; } = null!;
        public string Country { get; set; } = null!;

        public ICollection<PersonPhone> PersonPhones { get; set; } = [];
    }
}