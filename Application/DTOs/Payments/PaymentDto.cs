namespace Application.DTOs.Payments;

public sealed class PaymentDto
{
    public int Id { get; init; }
    public int InvoiceId { get; init; }
    public int PaymentMethodId { get; init; }
    public string PaymentMethodName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime PaidAt { get; init; }
    public string? Reference { get; init; }
}
