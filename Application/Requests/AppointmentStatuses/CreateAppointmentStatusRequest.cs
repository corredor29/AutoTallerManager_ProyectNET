using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.AppointmentStatuses
{
    public sealed class CreateAppointmentStatusRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; init; } = string.Empty;
    }
}