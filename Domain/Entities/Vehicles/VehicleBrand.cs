using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Vehicles.VehicleBrand;

namespace Domain.Entities.Vehicles
{

    public sealed class VehicleBrand : BaseEntity
    {
        public BrandName BrandName { get; private set; } = null!;

        public ICollection<VehicleModel> Models { get; private set; } = [];

        private VehicleBrand() { }

        public VehicleBrand(BrandName brandName)
        {
            BrandName = brandName ?? throw new ArgumentNullException(nameof(brandName));
        }

        public void Update(BrandName brandName)
        {
            BrandName = brandName ?? throw new ArgumentNullException(nameof(brandName));
        }
    }
}