namespace Application.DTOs.InvoiceDetails;

public sealed class InvoiceDetailDto
{
    public int Id { get; init; }
    public int InvoiceId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
