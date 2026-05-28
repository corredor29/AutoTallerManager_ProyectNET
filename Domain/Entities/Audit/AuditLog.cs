using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Users;
using Domain.ValueObject.Audit.AuditLog;
namespace Domain.Entities.Audit
{
    public sealed class AuditLog : BaseEntity
    {
        public int                UserId            { get; private set; }
        public int                AuditActionTypeId { get; private set; }
        public AffectedEntityName AffectedEntity    { get; private set; } = null!;
        public int                AffectedRecordId  { get; private set; }
        public DateTime           OccurredAt        { get; private set; }
        public AuditDescription   Description       { get; private set; } = null!;

        public User            User            { get; private set; } = null!;
        public AuditActionType AuditActionType { get; private set; } = null!;

        private AuditLog() { }

        public AuditLog(int userId, int auditActionTypeId,
                        AffectedEntityName affectedEntity, int affectedRecordId,
                        AuditDescription description)
        {
            UserId            = userId            > 0 ? userId            : throw new ArgumentException("UserId must be greater than 0.");
            AuditActionTypeId = auditActionTypeId > 0 ? auditActionTypeId : throw new ArgumentException("AuditActionTypeId must be greater than 0.");
            AffectedEntity    = affectedEntity ?? throw new ArgumentNullException(nameof(affectedEntity));
            AffectedRecordId  = affectedRecordId  > 0 ? affectedRecordId  : throw new ArgumentException("AffectedRecordId must be greater than 0.");
            Description       = description    ?? throw new ArgumentNullException(nameof(description));
            OccurredAt        = DateTime.UtcNow;
        }
    }
}