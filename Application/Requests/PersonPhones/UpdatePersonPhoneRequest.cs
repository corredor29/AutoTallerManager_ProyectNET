using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.PersonPhones
{
    public sealed class UpdatePersonPhoneRequest
    {
        [Required]
        public int PhoneCodeId { get; init; }

        [Required]
        [StringLength(30)]
        public string PhoneNumber { get; init; } = string.Empty;

        public bool IsPrimary { get; init; }
    }
}