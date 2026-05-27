using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Vehicles
{
    public class VehicleModel : BaseEntity
    {
        public int    BrandId   { get; set; }
        public string ModelName { get; set; } = null!;

        public VehicleBrand          Brand    { get; set; } = null!;
        public ICollection<Vehicle>  Vehicles { get; set; } = default!;
    }
}