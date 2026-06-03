using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Notifications
{
    public sealed class NotificationDto
    {
        public string Type        { get; init; } = string.Empty;
        public string Entity      { get; init; } = string.Empty;
        public int    RecordId    { get; init; }
        public string Message     { get; init; } = string.Empty;
        public string UserName    { get; init; } = string.Empty;
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}