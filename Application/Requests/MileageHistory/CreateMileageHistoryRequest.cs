using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MileageHistory
{
    public sealed class CreateMileageHistoryRequest
    {
        [Required]
        public int VehicleId { get; init; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Mileage cannot be negative.")]
        public int Mileage { get; init; }

        [StringLength(500)]
        public string? Notes { get; init; }
    }
}