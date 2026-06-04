using Domain.Entities.Parts;
using Domain.ValueObject.Parts.Part;

namespace AutoTallerManager.Tests.Domain;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class PartStockTests
{
    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Constructor_ZeroStock_IsValid()
    {
        var stock = new PartStock(0);
        stock.Value.Should().Be(0);
        stock.IsEmpty.Should().BeTrue();
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Constructor_PositiveStock_IsValid()
    {
        var stock = new PartStock(100);
        stock.Value.Should().Be(100);
        stock.IsEmpty.Should().BeFalse();
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Constructor_NegativeStock_ThrowsArgumentException()
    {
        var act = () => new PartStock(-1);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*negative*");
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Part_AddStock_IncreasesStockCorrectly()
    {
        var part = BuildPart(stock: 5, minStock: 2);

        part.AddStock(new PartStock(3));

        part.Stock.Value.Should().Be(8);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Part_RemoveStock_DecreasesStockCorrectly()
    {
        var part = BuildPart(stock: 10, minStock: 2);

        part.RemoveStock(new PartStock(4));

        part.Stock.Value.Should().Be(6);
        part.IsBelowMinStock().Should().BeFalse();
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Part_RemoveStock_InsufficientStock_ThrowsInvalidOperationException()
    {
        var part = BuildPart(stock: 3);

        var act = () => part.RemoveStock(new PartStock(5));

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Insufficient stock*");
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Part_IsBelowMinStock_ReturnsTrueWhenStockDropsBelowMinimum()
    {
        var part = BuildPart(stock: 3, minStock: 5);

        part.IsBelowMinStock().Should().BeTrue();
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void Part_IsBelowMinStock_ReturnsFalseWhenStockIsAboveMinimum()
    {
        var part = BuildPart(stock: 10, minStock: 5);

        part.IsBelowMinStock().Should().BeFalse();
    }

    // Construye un objeto de prueba con valores validos por defecto.
    private static Part BuildPart(int stock = 10, int minStock = 2) =>
        new(
            partCategoryId: 1,
            code:           new PartCode("TST-001"),
            description:    new PartDescription("Test part"),
            stock:          new PartStock(stock),
            minStock:       new PartMinStock(minStock),
            unitPrice:      new PartUnitPrice(50m));
}
