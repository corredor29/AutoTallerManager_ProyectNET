namespace Application.Requests.Invoices;

public sealed class CreateInvoiceRequest
{
    public int ServiceOrderId { get; init; }
    public int? QuotationId { get; init; }
    public decimal LaborCost { get; init; }
    public decimal Tax { get; init; }
    public bool DiagnosisOnlyCharged { get; init; }
}
