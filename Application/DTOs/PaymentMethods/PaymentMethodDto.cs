namespace Application.DTOs.PaymentMethods;

public sealed class PaymentMethodDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
