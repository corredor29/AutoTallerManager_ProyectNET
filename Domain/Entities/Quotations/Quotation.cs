using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.ServiceOrders;
using Domain.Entities.Users;
using Domain.Entities.Invoices;
using Domain.ValueObject.Quotations.Quotation;
namespace Domain.Entities.Quotations
{
public sealed class Quotation : BaseEntity
{
    public int               ServiceOrderId    { get; private set; }
    public int               CreatedByUserId   { get; private set; }
    public int               QuotationStatusId { get; private set; }
    public DateTime          CreatedAt         { get; private set; }
    public DateTime?         RespondedAt       { get; private set; }
    public LaborCost         LaborCost         { get; private set; } = null!;
    public QuotationSubtotal Subtotal          { get; private set; } = null!;
    public QuotationTotal    Total             { get; private set; } = null!;
    public RejectionReason   RejectionReason   { get; private set; } = null!;
    public QuotationNotes    Notes             { get; private set; } = null!;

    public ServiceOrder                 ServiceOrder    { get; private set; } = null!;
    public User                         CreatedByUser   { get; private set; } = null!;
    public QuotationStatus              QuotationStatus { get; private set; } = null!;
    public ICollection<QuotationDetail> Details         { get; private set; } = [];
    public Invoice?                     Invoice         { get; private set; }

    private Quotation() { }

    public Quotation(int serviceOrderId, int createdByUserId, int quotationStatusId,
                     LaborCost laborCost, QuotationSubtotal subtotal,
                     QuotationTotal total, QuotationNotes notes)
    {
        ServiceOrderId    = serviceOrderId    > 0 ? serviceOrderId    : throw new ArgumentException("ServiceOrderId must be greater than 0.");
        CreatedByUserId   = createdByUserId   > 0 ? createdByUserId   : throw new ArgumentException("CreatedByUserId must be greater than 0.");
        QuotationStatusId = quotationStatusId > 0 ? quotationStatusId : throw new ArgumentException("QuotationStatusId must be greater than 0.");
        LaborCost         = laborCost  ?? throw new ArgumentNullException(nameof(laborCost));
        Subtotal          = subtotal   ?? throw new ArgumentNullException(nameof(subtotal));
        Total             = total      ?? throw new ArgumentNullException(nameof(total));
        Notes             = notes      ?? throw new ArgumentNullException(nameof(notes));
        RejectionReason   = new RejectionReason(null);
        CreatedAt         = DateTime.UtcNow;
    }

    public void Accept()
    {
        RespondedAt = DateTime.UtcNow;
    }

    public void Reject(RejectionReason reason)
    {
        RejectionReason = reason ?? throw new ArgumentNullException(nameof(reason));
        RespondedAt     = DateTime.UtcNow;
    }

    public void Update(LaborCost laborCost, QuotationSubtotal subtotal,
                       QuotationTotal total, QuotationNotes notes)
    {
        LaborCost = laborCost ?? throw new ArgumentNullException(nameof(laborCost));
        Subtotal  = subtotal  ?? throw new ArgumentNullException(nameof(subtotal));
        Total     = total     ?? throw new ArgumentNullException(nameof(total));
        Notes     = notes     ?? throw new ArgumentNullException(nameof(notes));
    }
}
}