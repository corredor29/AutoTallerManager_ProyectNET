namespace Application.DTOs.ServiceOrderParts;

public sealed class ServiceOrderPartDto
{
    public int Id { get; init; }
    public int ServiceOrderId { get; init; }
    public int PartId { get; init; }
    public string PartCode { get; init; } = string.Empty;
    public string PartDescription { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal AppliedUnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}
