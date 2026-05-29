namespace Application.DTOs.Quotations;

public sealed class QuotationDto
{
    public int Id { get; init; }
    public int ServiceOrderId { get; init; }
    public int CreatedByUserId { get; init; }
    public string CreatedByUserName { get; init; } = string.Empty;
    public int QuotationStatusId { get; init; }
    public string QuotationStatusName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? RespondedAt { get; init; }
    public decimal LaborCost { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Total { get; init; }
    public string? RejectionReason { get; init; }
    public string? Notes { get; init; }
}
