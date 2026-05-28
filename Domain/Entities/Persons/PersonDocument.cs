using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Persons.PersonDocument;
namespace Domain.Entities.Persons
{
    public sealed class PersonDocument : BaseEntity
    {
        public int            PersonId       { get; private set; }
        public int            DocumentTypeId { get; private set; }
        public DocumentNumber DocumentNumber { get; private set; } = null!;
        public bool           IsPrimary      { get; private set; }

        public Person       Person       { get; private set; } = null!;
        public DocumentType DocumentType { get; private set; } = null!;

        private PersonDocument() { }

        public PersonDocument(int personId, int documentTypeId, DocumentNumber documentNumber, bool isPrimary = false)
        {
            PersonId       = personId       > 0 ? personId       : throw new ArgumentException("PersonId must be greater than 0.");
            DocumentTypeId = documentTypeId > 0 ? documentTypeId : throw new ArgumentException("DocumentTypeId must be greater than 0.");
            DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
            IsPrimary      = isPrimary;
        }

        public void Update(DocumentNumber documentNumber, bool isPrimary)
        {
            DocumentNumber = documentNumber ?? throw new ArgumentNullException(nameof(documentNumber));
            IsPrimary      = isPrimary;
        }

        public void SetAsPrimary()   => IsPrimary = true;
        public void SetAsSecondary() => IsPrimary = false;
    }
}