namespace Application.Requests.Invoices;

public sealed class UpdateInvoiceRequest
{
    public decimal LaborCost { get; init; }
    public decimal Tax { get; init; }
    public bool DiagnosisOnlyCharged { get; init; }
}
