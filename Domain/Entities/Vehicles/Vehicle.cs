using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Appointments;
using Domain.Entities.ServiceOrders;
using Domain.ValueObject.Vehicles.Vehicle;
namespace Domain.Entities.Vehicles
{

public sealed class Vehicle : BaseEntity
{
    public int            ModelId            { get; private set; }
    public int?           ColorId            { get; private set; }
    public int?           FuelTypeId         { get; private set; }
    public int?           TransmissionTypeId { get; private set; }
    public VinNumber      VIN                { get; private set; } = null!;
    public VehicleYear    Year               { get; private set; } = null!;
    public VehicleMileage Mileage            { get; private set; } = null!;
    public LicensePlate?  LicensePlate       { get; private set; }

    public VehicleModel                        Model            { get; private set; } = null!;
    public VehicleColor?                       Color            { get; private set; }
    public FuelType?                           FuelType         { get; private set; }
    public TransmissionType?                   TransmissionType { get; private set; }
    public ICollection<VehicleOwnershipHistory> Ownerships       { get; private set; } = [];
    public ICollection<MileageHistory>          MileageHistories { get; private set; } = [];
    public ICollection<ServiceOrder>            ServiceOrders    { get; private set; } = [];
    public ICollection<Appointment>             Appointments     { get; private set; } = [];

    private Vehicle() { }

    public Vehicle(int modelId, VinNumber vin, VehicleYear year, VehicleMileage mileage,
                   int? colorId = null, int? fuelTypeId = null, int? transmissionTypeId = null,
                   LicensePlate? licensePlate = null)
    {
        ModelId            = modelId > 0 ? modelId : throw new ArgumentException("ModelId must be greater than 0.");
        VIN                = vin          ?? throw new ArgumentNullException(nameof(vin));
        Year               = year         ?? throw new ArgumentNullException(nameof(year));
        Mileage            = mileage      ?? throw new ArgumentNullException(nameof(mileage));
        ColorId            = colorId;
        FuelTypeId         = fuelTypeId;
        TransmissionTypeId = transmissionTypeId;
        LicensePlate       = licensePlate;
    }

    public void Update(VehicleYear year, VehicleMileage mileage,
                       int? colorId, int? fuelTypeId, int? transmissionTypeId,
                       LicensePlate? licensePlate)
    {
        Year               = year    ?? throw new ArgumentNullException(nameof(year));
        Mileage            = mileage ?? throw new ArgumentNullException(nameof(mileage));
        ColorId            = colorId;
        FuelTypeId         = fuelTypeId;
        TransmissionTypeId = transmissionTypeId;
        LicensePlate       = licensePlate;
    }

    public void UpdateMileage(VehicleMileage mileage)
    {
        Mileage = mileage ?? throw new ArgumentNullException(nameof(mileage));
    }
}
}