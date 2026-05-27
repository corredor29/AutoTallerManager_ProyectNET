using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Users;
namespace Domain.Entities.Suppliers
{
    public class PurchaseOrder : BaseEntity
    {
        public int       SupplierId            { get; set; }
        public int       UserId                { get; set; }
        public int       PurchaseOrderStatusId { get; set; }
        public DateTime  OrderedAt             { get; set; } = DateTime.UtcNow;
        public DateTime? ReceivedAt            { get; set; }
        public decimal   Total                 { get; set; } = 0;
        public string?   Notes                 { get; set; }

        public Supplier                          Supplier { get; set; } = null!;
        public User                              User     { get; set; } = null!;
        public PurchaseOrderStatus               Status   { get; set; } = null!;
        public ICollection<PurchaseOrderDetail>  Details  { get; set; } = [];
    }
}