using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Infrastructure.Hubs
{
    [Authorize]
    public sealed class NotificationHub : Hub { }
}