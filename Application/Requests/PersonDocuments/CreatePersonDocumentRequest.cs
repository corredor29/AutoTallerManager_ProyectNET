using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.PersonDocuments
    {public sealed class CreatePersonDocumentRequest
    {
        [Required]
        public int PersonId { get; init; }

        [Required]
        public int DocumentTypeId { get; init; }

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; init; } = string.Empty;

        public bool IsPrimary { get; init; } = false;
    }
}