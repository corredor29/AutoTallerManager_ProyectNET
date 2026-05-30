using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.PersonPhones
{
    public sealed class PersonPhoneDto
    {
        public int    Id          { get; init; }
        public int    PersonId    { get; init; }
        public int    PhoneCodeId { get; init; }
        public string PhoneNumber { get; init; } = string.Empty;
        public string PhoneCode   { get; init; } = string.Empty;
        public string Country     { get; init; } = string.Empty;
        public string FullPhone   { get; init; } = string.Empty;
        public bool   IsPrimary   { get; init; }
    }
}