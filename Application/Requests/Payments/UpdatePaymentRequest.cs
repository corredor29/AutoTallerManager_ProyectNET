namespace Application.Requests.Payments;

public sealed class UpdatePaymentRequest
{
    public decimal Amount { get; init; }
    public string? Reference { get; init; }
}
