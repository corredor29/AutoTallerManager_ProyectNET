using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PurchaseOrderStatusEnum
    {
        Pending   = 1,
        Sent      = 2,
        Received  = 3,
        Cancelled = 4
    }
}