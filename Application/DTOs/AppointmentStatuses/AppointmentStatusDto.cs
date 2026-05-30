using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.AppointmentStatuses
{
    public sealed class AppointmentStatusDto
    {
        public int    Id   { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}