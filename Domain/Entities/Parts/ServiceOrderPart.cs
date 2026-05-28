using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.Parts.ServiceOrderPart;
namespace Domain.Entities.Parts
{
public sealed class ServiceOrderPart : BaseEntity
{
    public int                       ServiceOrderId   { get; private set; }
    public int                       PartId           { get; private set; }
    public ServiceOrderPartQuantity  Quantity         { get; private set; } = null!;
    public ServiceOrderPartUnitPrice AppliedUnitPrice { get; private set; } = null!;

    public ServiceOrder ServiceOrder { get; private set; } = null!;
    public Part         Part         { get; private set; } = null!;

    private ServiceOrderPart() { }

    public ServiceOrderPart(int serviceOrderId, int partId,
                            ServiceOrderPartQuantity quantity,
                            ServiceOrderPartUnitPrice appliedUnitPrice)
    {
        ServiceOrderId   = serviceOrderId > 0 ? serviceOrderId : throw new ArgumentException("ServiceOrderId must be greater than 0.");
        PartId           = partId         > 0 ? partId         : throw new ArgumentException("PartId must be greater than 0.");
        Quantity         = quantity         ?? throw new ArgumentNullException(nameof(quantity));
        AppliedUnitPrice = appliedUnitPrice ?? throw new ArgumentNullException(nameof(appliedUnitPrice));
    }

    public void Update(ServiceOrderPartQuantity quantity, ServiceOrderPartUnitPrice appliedUnitPrice)
    {
        Quantity         = quantity         ?? throw new ArgumentNullException(nameof(quantity));
        AppliedUnitPrice = appliedUnitPrice ?? throw new ArgumentNullException(nameof(appliedUnitPrice));
    }
}
}