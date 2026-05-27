using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Audit
{
    public class AuditActionType : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<AuditLog> AuditLogs { get; set; } = [];
    }
}