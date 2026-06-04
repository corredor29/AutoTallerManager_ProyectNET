using Application.Requests.Customers;

namespace AutoTallerManager.Tests.Application;

// Conjunto de pruebas automatizadas para validar este comportamiento del sistema.
public sealed class PagedQueryTests
{
    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void NormalizedPageNumber_ReturnsOne_WhenPageNumberIsLessThanOne()
    {
        var request = new GetCustomersRequest { PageNumber = 0 };
        request.NormalizedPageNumber.Should().Be(1);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void NormalizedPageNumber_ReturnsValue_WhenPageNumberIsValid()
    {
        var request = new GetCustomersRequest { PageNumber = 3 };
        request.NormalizedPageNumber.Should().Be(3);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void NormalizedPageSize_ReturnsDefault_WhenPageSizeIsLessThanOne()
    {
        var request = new GetCustomersRequest { PageSize = 0 };
        request.NormalizedPageSize.Should().Be(10);
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void NormalizedPageSize_ReturnsMax_WhenPageSizeExceedsLimit()
    {
        var request = new GetCustomersRequest { PageSize = 500 };
        request.NormalizedPageSize.Should().Be(50); // MaxPageSize = 50
    }

    // Verifica el escenario cubierto por este caso de prueba.
    [Fact]
    public void NormalizedPageSize_ReturnsValue_WhenPageSizeIsValid()
    {
        var request = new GetCustomersRequest { PageSize = 20 };
        request.NormalizedPageSize.Should().Be(20);
    }
}
