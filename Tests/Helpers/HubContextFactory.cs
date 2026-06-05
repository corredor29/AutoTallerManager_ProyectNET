using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace AutoTallerManager.Tests.Helpers;

public static class HubContextFactory
{
    public static IHubContext<NotificationHub> Create()
    {
        var clientProxy = new Mock<IClientProxy>();
        var clients = new Mock<IHubClients>();
        var groupManager = new Mock<IGroupManager>();
        var hubContext = new Mock<IHubContext<NotificationHub>>();

        clientProxy
            .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        clients.Setup(x => x.All).Returns(clientProxy.Object);
        hubContext.SetupGet(x => x.Clients).Returns(clients.Object);
        hubContext.SetupGet(x => x.Groups).Returns(groupManager.Object);

        return hubContext.Object;
    }
}
