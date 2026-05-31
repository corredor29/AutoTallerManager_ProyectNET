using Domain.Entities.Parts;
using Domain.ValueObject.Parts.Part;

namespace AutoTallerManager.Tests.Domain;

public sealed class PartStockTests
{
    [Fact]
    public void Constructor_ZeroStock_IsValid()
    {
        var stock = new PartStock(0);
        stock.Value.Should().Be(0);
        stock.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Constructor_PositiveStock_IsValid()
    {
        var stock = new PartStock(100);
        stock.Value.Should().Be(100);
        stock.IsEmpty.Should().BeFalse();
    }

    [Fact]
    public void Constructor_NegativeStock_ThrowsArgumentException()
    {
        var act = () => new PartStock(-1);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*negative*");
    }

    [Fact]
    public void Part_AddStock_IncreasesStockCorrectly()
    {
        var part = BuildPart(stock: 5, minStock: 2);

        part.AddStock(new PartStock(3));

        part.Stock.Value.Should().Be(8);
    }

    [Fact]
    public void Part_RemoveStock_DecreasesStockCorrectly()
    {
        var part = BuildPart(stock: 10, minStock: 2);

        part.RemoveStock(new PartStock(4));

        part.Stock.Value.Should().Be(6);
        part.IsBelowMinStock().Should().BeFalse();
    }

    [Fact]
    public void Part_RemoveStock_InsufficientStock_ThrowsInvalidOperationException()
    {
        var part = BuildPart(stock: 3);

        var act = () => part.RemoveStock(new PartStock(5));

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void Part_IsBelowMinStock_ReturnsTrueWhenStockDropsBelowMinimum()
    {
        var part = BuildPart(stock: 3, minStock: 5);

        part.IsBelowMinStock().Should().BeTrue();
    }

    [Fact]
    public void Part_IsBelowMinStock_ReturnsFalseWhenStockIsAboveMinimum()
    {
        var part = BuildPart(stock: 10, minStock: 5);

        part.IsBelowMinStock().Should().BeFalse();
    }

    private static Part BuildPart(int stock = 10, int minStock = 2) =>
        new(
            partCategoryId: 1,
            code:           new PartCode("TST-001"),
            description:    new PartDescription("Test part"),
            stock:          new PartStock(stock),
            minStock:       new PartMinStock(minStock),
            unitPrice:      new PartUnitPrice(50m));
}
