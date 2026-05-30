using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.VehicleBrands
{
    public sealed class VehicleBrandDto
    {
        public int    Id        { get; init; }
        public string BrandName { get; init; } = string.Empty;
    }
}