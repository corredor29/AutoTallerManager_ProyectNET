using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Users;
using Domain.ValueObject.Suppliers.PurchaseOrder;
namespace Domain.Entities.Suppliers
{

    public sealed class PurchaseOrder : BaseEntity
    {
        public int                 SupplierId            { get; private set; }
        public int                 UserId                { get; private set; }
        public int                 PurchaseOrderStatusId { get; private set; }
        public DateTime            OrderedAt             { get; private set; }
        public DateTime?           ReceivedAt            { get; private set; }
        public PurchaseOrderTotal  Total                 { get; private set; } = null!;
        public PurchaseOrderNotes  Notes                 { get; private set; } = null!;

        public Supplier                         Supplier { get; private set; } = null!;
        public User                             User     { get; private set; } = null!;
        public PurchaseOrderStatus              Status   { get; private set; } = null!;
        public ICollection<PurchaseOrderDetail> Details  { get; private set; } = [];

        private PurchaseOrder() { }

        public PurchaseOrder(int supplierId, int userId, int purchaseOrderStatusId,
                            PurchaseOrderTotal total, PurchaseOrderNotes notes)
        {
            SupplierId            = supplierId            > 0 ? supplierId            : throw new ArgumentException("SupplierId must be greater than 0.");
            UserId                = userId                > 0 ? userId                : throw new ArgumentException("UserId must be greater than 0.");
            PurchaseOrderStatusId = purchaseOrderStatusId > 0 ? purchaseOrderStatusId : throw new ArgumentException("PurchaseOrderStatusId must be greater than 0.");
            Total                 = total ?? throw new ArgumentNullException(nameof(total));
            Notes                 = notes ?? throw new ArgumentNullException(nameof(notes));
            OrderedAt             = DateTime.UtcNow;
        }

        public void Update(PurchaseOrderTotal total, PurchaseOrderNotes notes)
        {
            Total = total ?? throw new ArgumentNullException(nameof(total));
            Notes = notes ?? throw new ArgumentNullException(nameof(notes));
        }

        public void ChangeStatus(int purchaseOrderStatusId)
        {
            PurchaseOrderStatusId = purchaseOrderStatusId > 0 ? purchaseOrderStatusId : throw new ArgumentException("PurchaseOrderStatusId must be greater than 0.");
        }

        public void Receive()
        {
            ReceivedAt = DateTime.UtcNow;
        }
    }
}