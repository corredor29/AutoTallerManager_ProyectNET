using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.VehicleBrands
{
    public sealed class CreateVehicleBrandRequest
    {
        [Required]
        [StringLength(80)]
        public string BrandName { get; init; } = string.Empty;
    }
}