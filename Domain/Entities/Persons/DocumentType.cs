using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Persons.DocumentType;

namespace Domain.Entities.Persons
{
    public sealed class DocumentType : BaseEntity
    {
        public DocumentCode         Code { get; private set; } = null!;
        public DocumentTypeName     Name { get; private set; } = null!;

        public ICollection<PersonDocument> PersonDocuments { get; private set; } = [];

        private DocumentType() { }

        public DocumentType(DocumentCode code, DocumentTypeName name)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(DocumentCode code, DocumentTypeName name)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}