using Domain.ValueObject.Parts.Part;
using Domain.ValueObject.Parts.ServiceOrderPart;

namespace AutoTallerManager.Tests.Domain;

public sealed class PartUnitPriceTests
{
    [Fact]
    public void PartUnitPrice_ZeroValue_IsValid()
    {
        var price = new PartUnitPrice(0m);
        price.Value.Should().Be(0m);
    }

    [Fact]
    public void PartUnitPrice_PositiveValue_IsValid()
    {
        var price = new PartUnitPrice(99.99m);
        price.Value.Should().Be(99.99m);
    }

    [Fact]
    public void PartUnitPrice_NegativeValue_ThrowsArgumentException()
    {
        var act = () => new PartUnitPrice(-0.01m);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*negative*");
    }

    [Fact]
    public void ServiceOrderPartQuantity_ZeroOrNegative_ThrowsArgumentException()
    {
        var actZero = () => new ServiceOrderPartQuantity(0);
        var actNeg  = () => new ServiceOrderPartQuantity(-1);

        actZero.Should().Throw<ArgumentException>();
        actNeg.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ServiceOrderPartQuantity_PositiveValue_IsValid()
    {
        var quantity = new ServiceOrderPartQuantity(3);
        quantity.Value.Should().Be(3);
    }

    [Fact]
    public void PartMinStock_NegativeValue_ThrowsArgumentException()
    {
        var act = () => new PartMinStock(-1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void PartCode_EmptyValue_ThrowsArgumentException()
    {
        var act = () => new PartCode(string.Empty);
        act.Should().Throw<ArgumentException>();
    }
}
