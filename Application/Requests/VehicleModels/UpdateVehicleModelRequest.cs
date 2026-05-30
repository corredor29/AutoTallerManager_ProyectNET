using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.VehicleModels
{
public sealed class UpdateVehicleModelRequest
{
    [Required]
    public int BrandId { get; init; }

    [Required]
    [StringLength(80)]
    public string ModelName { get; init; } = string.Empty;
}
}