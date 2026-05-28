using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Parts;
using Domain.ValueObject.Suppliers.PurchaseOrderDetail;
namespace Domain.Entities.Suppliers
{

public sealed class PurchaseOrderDetail : BaseEntity
{
    public int                           PurchaseOrderId { get; private set; }
    public int                           PartId          { get; private set; }
    public PurchaseOrderDetailQuantity   Quantity        { get; private set; } = null!;
    public PurchaseOrderDetailUnitPrice  UnitPrice       { get; private set; } = null!;

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;
    public Part          Part          { get; private set; } = null!;

    private PurchaseOrderDetail() { }

    public PurchaseOrderDetail(int purchaseOrderId, int partId,
                               PurchaseOrderDetailQuantity quantity,
                               PurchaseOrderDetailUnitPrice unitPrice)
    {
        PurchaseOrderId = purchaseOrderId > 0 ? purchaseOrderId : throw new ArgumentException("PurchaseOrderId must be greater than 0.");
        PartId          = partId          > 0 ? partId          : throw new ArgumentException("PartId must be greater than 0.");
        Quantity        = quantity  ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice       = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    public void Update(PurchaseOrderDetailQuantity quantity, PurchaseOrderDetailUnitPrice unitPrice)
    {
        Quantity  = quantity  ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }
}
}