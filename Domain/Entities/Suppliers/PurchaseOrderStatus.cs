using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Suppliers
{
    public class PurchaseOrderStatus : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
    }
}