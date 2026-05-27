using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Persons
{
    public class DocumentType : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public ICollection<PersonDocument> PersonDocuments { get; set; } = default!;
    }
}