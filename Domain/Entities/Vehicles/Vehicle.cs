using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Appointments;
using Domain.Entities.ServiceOrders;
namespace Domain.Entities.Vehicles
{
    public class Vehicle : BaseEntity
    {
        public int     ModelId            { get; set; }
        public int?    ColorId            { get; set; }
        public int?    FuelTypeId         { get; set; }
        public int?    TransmissionTypeId { get; set; }
        public string  VIN                { get; set; } = null!;
        public short   Year               { get; set; }
        public int     Mileage            { get; set; } = 0;
        public string? LicensePlate       { get; set; }

        public VehicleModel                        Model             { get; set; } = null!;
        public VehicleColor?                       Color             { get; set; }
        public FuelType?                           FuelType          { get; set; }
        public TransmissionType?                   TransmissionType  { get; set; }
        public ICollection<VehicleOwnershipHistory> Ownerships        { get; set; } = default!;
        public ICollection<MileageHistory>          MileageHistories  { get; set; } = default!;
        public ICollection<ServiceOrder>            ServiceOrders     { get; set; } = default!;
        public ICollection<Appointment>             Appointments      { get; set; } = default!;
    }
}