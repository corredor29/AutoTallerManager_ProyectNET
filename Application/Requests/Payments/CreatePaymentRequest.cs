namespace Application.Requests.Payments;

public sealed class CreatePaymentRequest
{
    public int InvoiceId { get; init; }
    public int PaymentMethodId { get; init; }
    public decimal Amount { get; init; }
    public string? Reference { get; init; }
}
