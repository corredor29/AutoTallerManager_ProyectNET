using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Invoices.InvoiceDetail;
namespace Domain.Entities.Invoices
{
public sealed class InvoiceDetail : BaseEntity
{
    public int                      InvoiceId   { get; private set; }
    public InvoiceDetailDescription Description { get; private set; } = null!;
    public InvoiceDetailQuantity    Quantity    { get; private set; } = null!;
    public InvoiceDetailUnitPrice   UnitPrice   { get; private set; } = null!;

    public Invoice Invoice { get; private set; } = null!;

    private InvoiceDetail() { }

    public InvoiceDetail(int invoiceId, InvoiceDetailDescription description,
                         InvoiceDetailQuantity quantity, InvoiceDetailUnitPrice unitPrice)
    {
        InvoiceId   = invoiceId > 0 ? invoiceId : throw new ArgumentException("InvoiceId must be greater than 0.");
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity    = quantity    ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice   = unitPrice   ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    public void Update(InvoiceDetailDescription description,
                       InvoiceDetailQuantity quantity,
                       InvoiceDetailUnitPrice unitPrice)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Quantity    = quantity    ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice   = unitPrice   ?? throw new ArgumentNullException(nameof(unitPrice));
    }
}
}