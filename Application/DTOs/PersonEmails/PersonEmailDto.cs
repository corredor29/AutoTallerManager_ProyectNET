using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.PersonEmails
{

    public sealed class PersonEmailDto
    {
        public int    Id            { get; init; }
        public int    PersonId      { get; init; }
        public int    EmailDomainId { get; init; }
        public string EmailUser     { get; init; } = string.Empty;
        public string Domain        { get; init; } = string.Empty;
        public string FullEmail     { get; init; } = string.Empty;
        public bool   IsPrimary     { get; init; }
    }
}