using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Vehicles
{
    public class MileageHistory : BaseEntity
    {
        public int      VehicleId  { get; set; }
        public int      Mileage    { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public string?  Notes      { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }
}