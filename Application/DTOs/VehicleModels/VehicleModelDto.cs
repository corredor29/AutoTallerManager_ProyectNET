using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.VehicleModels
{
    public sealed class VehicleModelDto
    {
        public int    Id        { get; init; }
        public int    BrandId   { get; init; }
        public string ModelName { get; init; } = string.Empty;
        public string BrandName { get; init; } = string.Empty;
    }
}