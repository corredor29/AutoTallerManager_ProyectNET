using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Customers;

namespace Domain.Entities.Vehicles
{
    public class VehicleOwnershipHistory : BaseEntity
    {
        public int       VehicleId  { get; set; }
        public int       CustomerId { get; set; }
        public DateOnly  StartDate  { get; set; }
        public DateOnly? EndDate    { get; set; }

        public Vehicle  Vehicle  { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}