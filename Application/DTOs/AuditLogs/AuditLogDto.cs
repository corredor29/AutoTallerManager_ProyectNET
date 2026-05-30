using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.AuditLogs
{
    public sealed class AuditLogDto
    {
        public int      Id               { get; init; }
        public int      UserId           { get; init; }
        public int      AuditActionTypeId { get; init; }
        public string   AffectedEntity   { get; init; } = string.Empty;
        public int      AffectedRecordId { get; init; }
        public DateTime OccurredAt       { get; init; }
        public string?  Description      { get; init; }
        public string   ActionName       { get; init; } = string.Empty;
        public string   UserFullName     { get; init; } = string.Empty;
    }
}