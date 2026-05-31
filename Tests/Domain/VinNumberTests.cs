using Domain.ValueObject.Vehicles.Vehicle;

namespace AutoTallerManager.Tests.Domain;

public sealed class VinNumberTests
{
    [Fact]
    public void Constructor_ValidVin17Chars_CreatesVinNumber()
    {
        var vin = new VinNumber("1HGCM82633A004352");
        vin.Value.Should().Be("1HGCM82633A004352");
    }

    [Theory]
    [InlineData("1HGCM82633A00435")]  // 16 chars
    [InlineData("1HGCM82633A0043521")] // 18 chars
    public void Constructor_WrongLength_ThrowsArgumentException(string value)
    {
        var act = () => new VinNumber(value);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*17 characters*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyOrWhitespace_ThrowsArgumentException(string value)
    {
        var act = () => new VinNumber(value);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*empty*");
    }

    [Fact]
    public void Constructor_VinWithSpecialCharacters_ThrowsArgumentException()
    {
        var act = () => new VinNumber("1HGCM826-3A004352");
        act.Should().Throw<ArgumentException>()
           .WithMessage("*letters and digits*");
    }

    [Fact]
    public void Constructor_LowercaseVin_StoresUppercase()
    {
        var vin = new VinNumber("1hgcm82633a004352");
        vin.Value.Should().Be("1HGCM82633A004352");
    }
}
