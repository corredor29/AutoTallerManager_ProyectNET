using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
namespace Domain.Entities.Persons
{
    public class EmailDomain : BaseEntity
    {
        public string Domain { get; set; } = null!;

        public ICollection<PersonEmail> PersonEmails { get; set; } = default!;
    }
}