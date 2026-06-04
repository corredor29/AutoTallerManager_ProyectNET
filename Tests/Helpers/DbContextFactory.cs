namespace AutoTallerManager.Tests.Helpers;

internal static class DbContextFactory
{
    // Crea una instancia lista para usar dentro de los escenarios de prueba.
    internal static AutoTallerDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AutoTallerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        return new AutoTallerDbContext(options, mockAccessor.Object);
    }
}
