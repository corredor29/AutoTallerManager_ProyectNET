using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Persons
{
    public class PersonDocument : BaseEntity
    {
        public int    PersonId       { get; set; }
        public int    DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public bool   IsPrimary      { get; set; } = false;

        public Person       Person       { get; set; } = null!;
        public DocumentType DocumentType { get; set; } = null!;
    }
}