using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Vehicles.VehicleColor;
namespace Domain.Entities.Vehicles
{
    public sealed class VehicleColor : BaseEntity
    {
        public ColorName Name { get; private set; } = null!;

        public ICollection<Vehicle> Vehicles { get; private set; } = [];

        private VehicleColor() { }

        public VehicleColor(ColorName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(ColorName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}