using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.PersonEmails
{
    public sealed class UpdatePersonEmailRequest
    {
        [Required]
        public int EmailDomainId { get; init; }

        [Required]
        [StringLength(100)]
        public string EmailUser { get; init; } = string.Empty;

        public bool IsPrimary { get; init; }
    }
}