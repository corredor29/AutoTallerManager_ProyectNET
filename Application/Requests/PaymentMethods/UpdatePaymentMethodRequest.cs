namespace Application.Requests.PaymentMethods;

public sealed class UpdatePaymentMethodRequest
{
    public string Name { get; init; } = string.Empty;
}
