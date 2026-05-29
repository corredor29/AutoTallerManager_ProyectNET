using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Quotations;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.Invoices.Invoice;
namespace Domain.Entities.Invoices
{
public sealed class Invoice : BaseEntity
{
    public int              ServiceOrderId       { get; private set; }
    public int?             QuotationId          { get; private set; }
    public DateTime         IssuedAt             { get; private set; }
    public InvoiceLaborCost LaborCost            { get; private set; } = null!;
    public InvoiceSubtotal  Subtotal             { get; private set; } = null!;
    public InvoiceTax       Tax                  { get; private set; } = null!;
    public InvoiceTotal     Total                { get; private set; } = null!;
    public bool             DiagnosisOnlyCharged { get; private set; }

    public ServiceOrder              ServiceOrder { get; private set; } = null!;
    public Quotation?                Quotation    { get; private set; }
    public ICollection<InvoiceDetail> Details     { get; private set; } = [];
    public ICollection<Payment>       Payments    { get; private set; } = [];

    private Invoice() { }

    public Invoice(int serviceOrderId, InvoiceLaborCost laborCost,
                   InvoiceSubtotal subtotal, InvoiceTax tax,
                   InvoiceTotal total, bool diagnosisOnlyCharged = false,
                   int? quotationId = null)
    {
        ServiceOrderId       = serviceOrderId > 0 ? serviceOrderId : throw new ArgumentException("ServiceOrderId must be greater than 0.");
        LaborCost            = laborCost ?? throw new ArgumentNullException(nameof(laborCost));
        Subtotal             = subtotal  ?? throw new ArgumentNullException(nameof(subtotal));
        Tax                  = tax       ?? throw new ArgumentNullException(nameof(tax));
        Total                = total     ?? throw new ArgumentNullException(nameof(total));
        DiagnosisOnlyCharged = diagnosisOnlyCharged;
        QuotationId          = quotationId;
        IssuedAt             = DateTime.UtcNow;
    }

    public void Update(InvoiceLaborCost laborCost, InvoiceSubtotal subtotal,
                       InvoiceTax tax, InvoiceTotal total, bool diagnosisOnlyCharged)
    {
        LaborCost = laborCost ?? throw new ArgumentNullException(nameof(laborCost));
        Subtotal  = subtotal  ?? throw new ArgumentNullException(nameof(subtotal));
        Tax       = tax       ?? throw new ArgumentNullException(nameof(tax));
        Total     = total     ?? throw new ArgumentNullException(nameof(total));
        DiagnosisOnlyCharged = diagnosisOnlyCharged;
    }
}
}
