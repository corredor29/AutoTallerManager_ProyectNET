using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.VehicleOwnershipHistory
{
    public sealed class CloseOwnershipRequest
    {
        [Required]
        public DateOnly EndDate { get; init; }
    }
}