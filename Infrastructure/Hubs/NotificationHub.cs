using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Infrastructure.Hubs
{
    // Hub de SignalR protegido que sirve como punto de conexion para notificaciones en tiempo real.
    [Authorize]
    public sealed class NotificationHub : Hub { }
}
