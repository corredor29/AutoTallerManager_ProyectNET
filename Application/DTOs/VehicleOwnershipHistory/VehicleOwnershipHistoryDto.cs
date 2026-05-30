using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.VehicleOwnershipHistory
{
    public sealed class VehicleOwnershipHistoryDto
    {
        public int       Id         { get; init; }
        public int       VehicleId  { get; init; }
        public int       CustomerId { get; init; }
        public DateOnly  StartDate  { get; init; }
        public DateOnly? EndDate    { get; init; }
        public string    FirstName  { get; init; } = string.Empty;
        public string    LastName   { get; init; } = string.Empty;
        public string    VIN        { get; init; } = string.Empty;
        public bool      IsActive   => EndDate == null;
    }
}