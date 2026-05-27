using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Users;

namespace Domain.Entities.Audit
{
    public class AuditLog : BaseEntity
    {
        public int      UserId            { get; set; }
        public int      AuditActionTypeId { get; set; }
        public string   AffectedEntity    { get; set; } = null!;
        public int      AffectedRecordId  { get; set; }
        public DateTime OccurredAt        { get; set; } = DateTime.UtcNow;
        public string?  Description       { get; set; }

        public User            User            { get; set; } = null!;
        public AuditActionType AuditActionType { get; set; } = null!;
    }
}