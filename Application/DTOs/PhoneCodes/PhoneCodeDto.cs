using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.PhoneCodes
{
    public sealed class PhoneCodeDto
    {
        public int    Id      { get; init; }
        public string Code    { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
    }
}