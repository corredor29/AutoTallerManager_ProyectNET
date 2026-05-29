namespace Application.DTOs.Invoices;

public sealed class InvoiceDto
{
    public int Id { get; init; }
    public int ServiceOrderId { get; init; }
    public int? QuotationId { get; init; }
    public DateTime IssuedAt { get; init; }
    public decimal LaborCost { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
    public bool DiagnosisOnlyCharged { get; init; }
}
