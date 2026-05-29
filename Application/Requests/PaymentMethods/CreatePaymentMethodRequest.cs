namespace Application.Requests.PaymentMethods;

public sealed class CreatePaymentMethodRequest
{
    public string Name { get; init; } = string.Empty;
}
