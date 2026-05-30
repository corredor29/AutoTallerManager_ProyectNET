using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.DocumentTypes
{
    public sealed class UpdateDocumentTypeRequest
    {
        [Required]
        [StringLength(10)]
        public string Code { get; init; } = string.Empty;

        [Required]
        [StringLength(80)]
        public string Name { get; init; } = string.Empty;
    }
}