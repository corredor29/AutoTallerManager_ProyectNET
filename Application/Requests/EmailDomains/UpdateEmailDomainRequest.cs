using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.EmailDomains
{
    public sealed class UpdateEmailDomainRequest
    {
        [Required]
        [StringLength(100)]
        public string Domain { get; init; } = string.Empty;
    }
}