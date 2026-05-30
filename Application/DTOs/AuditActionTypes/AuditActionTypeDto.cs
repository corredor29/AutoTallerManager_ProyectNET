using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.AuditActionTypes
{
    public sealed class AuditActionTypeDto
    {
        public int    Id   { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}