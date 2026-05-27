using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
namespace Domain.Entities.Vehicles
{
    public class FuelType : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Vehicle> Vehicles { get; set; } = default!;
    }
}