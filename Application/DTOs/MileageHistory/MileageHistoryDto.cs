using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.MileageHistory
{
    public sealed class MileageHistoryDto
    {
        public int      Id         { get; init; }
        public int      VehicleId  { get; init; }
        public int      Mileage    { get; init; }
        public DateTime RecordedAt { get; init; }
        public string?  Notes      { get; init; }
        public string   VIN        { get; init; } = string.Empty;
    }
}