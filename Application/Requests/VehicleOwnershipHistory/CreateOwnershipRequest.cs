using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.VehicleOwnershipHistory
{
    public sealed class CreateOwnershipRequest
    {
        [Required]
        public int VehicleId { get; init; }

        [Required]
        public int CustomerId { get; init; }

        [Required]
        public DateOnly StartDate { get; init; }
    }
}