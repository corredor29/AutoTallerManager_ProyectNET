using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Suppliers.PurchaseOrderStatus;
namespace Domain.Entities.Suppliers
{

    public sealed class PurchaseOrderStatus : BaseEntity
    {
        public PurchaseOrderStatusName Name { get; private set; } = null!;

        public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = [];

        private PurchaseOrderStatus() { }

        public PurchaseOrderStatus(PurchaseOrderStatusName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(PurchaseOrderStatusName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}