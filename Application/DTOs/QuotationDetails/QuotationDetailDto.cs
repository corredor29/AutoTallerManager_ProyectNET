namespace Application.DTOs.QuotationDetails;

public sealed class QuotationDetailDto
{
    public int Id { get; init; }
    public int QuotationId { get; init; }
    public int PartId { get; init; }
    public string PartCode { get; init; } = string.Empty;
    public string PartDescription { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
