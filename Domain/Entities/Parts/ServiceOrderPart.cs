using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.ServiceOrders;

namespace Domain.Entities.Parts
{
    public class ServiceOrderPart : BaseEntity
    {
        public int     ServiceOrderId   { get; set; }
        public int     PartId           { get; set; }
        public int     Quantity         { get; set; }
        public decimal AppliedUnitPrice { get; set; }

        public ServiceOrder ServiceOrder { get; set; } = null!;
        public Part Part         { get; set; } = null!;
    }
}