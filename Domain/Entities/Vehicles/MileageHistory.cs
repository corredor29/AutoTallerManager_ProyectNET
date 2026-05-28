using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Vehicles.MileageHistory;
namespace Domain.Entities.Vehicles
{

public sealed class MileageHistory : BaseEntity
{
    public int           VehicleId  { get; private set; }
    public MileageRecord Mileage    { get; private set; } = null!;
    public DateTime      RecordedAt { get; private set; }
    public string?       Notes      { get; private set; }

    public Vehicle Vehicle { get; private set; } = null!;

    private MileageHistory() { }

    public MileageHistory(int vehicleId, MileageRecord mileage, string? notes = null)
    {
        VehicleId  = vehicleId > 0 ? vehicleId : throw new ArgumentException("VehicleId must be greater than 0.");
        Mileage    = mileage ?? throw new ArgumentNullException(nameof(mileage));
        RecordedAt = DateTime.UtcNow;
        Notes      = notes;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
}