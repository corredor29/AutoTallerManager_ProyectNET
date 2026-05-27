using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Vehicles
{
    public class VehicleBrand : BaseEntity
    {
        public string BrandName { get; set; } = null!;

        public ICollection<VehicleModel> Models { get; set; } = [];
    }
}