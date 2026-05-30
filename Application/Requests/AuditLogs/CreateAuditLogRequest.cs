using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.AuditLogs
{
    public sealed class CreateAuditLogRequest
    {
        [Required]
        public int UserId { get; init; }

        [Required]
        public int AuditActionTypeId { get; init; }

        [Required]
        [StringLength(100)]
        public string AffectedEntity { get; init; } = string.Empty;

        [Required]
        public int AffectedRecordId { get; init; }

        public string? Description { get; init; }
    }
}