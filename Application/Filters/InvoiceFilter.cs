namespace Application.Filters;

public sealed class InvoiceFilter
{
    public DateTime? DateFrom       { get; init; }
    public DateTime? DateTo         { get; init; }
    public int?      ServiceOrderId { get; init; }
    public int?      CustomerId     { get; init; }
    public string?   CustomerName   { get; init; }
}
