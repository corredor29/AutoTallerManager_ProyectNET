using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Vehicles.FuelType;

namespace Domain.Entities.Vehicles
{
    public sealed class FuelType : BaseEntity
    {
        public FuelTypeName Name { get; private set; } = null!;

        public ICollection<Vehicle> Vehicles { get; private set; } = [];

        private FuelType() { }

        public FuelType(FuelTypeName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(FuelTypeName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}