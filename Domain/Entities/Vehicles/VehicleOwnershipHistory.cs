using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Customers;
using Domain.ValueObject.Vehicles.VehicleOwnershipHistory;
namespace Domain.Entities.Vehicles
{

    public sealed class VehicleOwnershipHistory : BaseEntity
    {
        public int       VehicleId  { get; private set; }
        public int       CustomerId { get; private set; }
        public DateRange DateRange  { get; private set; } = null!;

        public Vehicle  Vehicle  { get; private set; } = null!;
        public Customer Customer { get; private set; } = null!;

        private VehicleOwnershipHistory() { }

        public VehicleOwnershipHistory(int vehicleId, int customerId, DateRange dateRange)
        {
            VehicleId  = vehicleId  > 0 ? vehicleId  : throw new ArgumentException("VehicleId must be greater than 0.");
            CustomerId = customerId > 0 ? customerId : throw new ArgumentException("CustomerId must be greater than 0.");
            DateRange  = dateRange ?? throw new ArgumentNullException(nameof(dateRange));
        }

        public void Close(DateOnly endDate)
        {
            DateRange = new DateRange(DateRange.StartDate, endDate);
        }
    }
}