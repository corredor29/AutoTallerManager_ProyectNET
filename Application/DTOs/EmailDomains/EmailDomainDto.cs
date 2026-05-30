using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.EmailDomains
{
    public sealed class EmailDomainDto
    {
        public int    Id     { get; init; }
        public string Domain { get; init; } = string.Empty;
    }
}