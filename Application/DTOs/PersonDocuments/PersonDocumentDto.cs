using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.PersonDocuments
{
    public sealed class PersonDocumentDto
    {
        public int    Id             { get; init; }
        public int    PersonId       { get; init; }
        public int    DocumentTypeId { get; init; }
        public string DocumentNumber { get; init; } = string.Empty;
        public bool   IsPrimary      { get; init; }
        public string DocumentType   { get; init; } = string.Empty;
        public string DocumentCode   { get; init; } = string.Empty;
    }
}