using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.PhoneCodes
{
    public sealed class UpdatePhoneCodeRequest
    {
        [Required]
        [StringLength(10)]
        public string Code { get; init; } = string.Empty;

        [Required]
        [StringLength(80)]
        public string Country { get; init; } = string.Empty;
    }
}