using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.PersonPhones
{
    public sealed class CreatePersonPhoneRequest
    {
        [Required]
        public int PersonId { get; init; }

        [Required]
        public int PhoneCodeId { get; init; }

        [Required]
        [StringLength(30)]
        public string PhoneNumber { get; init; } = string.Empty;

        public bool IsPrimary { get; init; } = false;
    }
}