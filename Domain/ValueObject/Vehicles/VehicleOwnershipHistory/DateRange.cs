using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.ValueObject.Vehicles.VehicleOwnershipHistory
{
    public record DateRange
    {
        public DateOnly  StartDate { get; }
        public DateOnly? EndDate   { get; }

        public DateRange(DateOnly startDate, DateOnly? endDate = null)
        {
            if (endDate.HasValue && endDate.Value < startDate)
                throw new ArgumentException("End date cannot be before start date.");

            if (startDate > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("Start date cannot be in the future.");

            StartDate = startDate;
            EndDate   = endDate;
        }

        public bool IsActive  => EndDate == null;
        public bool IsClosed  => EndDate != null;
    }
}