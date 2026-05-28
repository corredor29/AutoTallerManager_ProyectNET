using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Parts;
using Domain.ValueObject.Suppliers.PartSupplier;
namespace Domain.Entities.Suppliers
{

public sealed class PartSupplier : BaseEntity
{
    public int                  PartId        { get; private set; }
    public int                  SupplierId    { get; private set; }
    public PartSupplierPrice    PurchasePrice { get; private set; } = null!;
    public PartSupplierIsPrimary IsPrimary    { get; private set; } = null!;

    public Part     Part     { get; private set; } = null!;
    public Supplier Supplier { get; private set; } = null!;

    private PartSupplier() { }

    public PartSupplier(int partId, int supplierId,
                        PartSupplierPrice purchasePrice,
                        PartSupplierIsPrimary isPrimary)
    {
        PartId        = partId     > 0 ? partId     : throw new ArgumentException("PartId must be greater than 0.");
        SupplierId    = supplierId > 0 ? supplierId : throw new ArgumentException("SupplierId must be greater than 0.");
        PurchasePrice = purchasePrice ?? throw new ArgumentNullException(nameof(purchasePrice));
        IsPrimary     = isPrimary     ?? throw new ArgumentNullException(nameof(isPrimary));
    }

    public void Update(PartSupplierPrice purchasePrice, PartSupplierIsPrimary isPrimary)
    {
        PurchasePrice = purchasePrice ?? throw new ArgumentNullException(nameof(purchasePrice));
        IsPrimary     = isPrimary     ?? throw new ArgumentNullException(nameof(isPrimary));
    }

    public void SetAsPrimary()   => IsPrimary = PartSupplierIsPrimary.Primary;
    public void SetAsSecondary() => IsPrimary = PartSupplierIsPrimary.Secondary;
}
}