using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Audit.AuditActionType;
namespace Domain.Entities.Audit
{
    public sealed class AuditActionType : BaseEntity
    {
        public AuditActionTypeName Name { get; private set; } = null!;

        public ICollection<AuditLog> AuditLogs { get; private set; } = [];

        private AuditActionType() { }

        public AuditActionType(AuditActionTypeName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(AuditActionTypeName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}