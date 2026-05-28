using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Vehicles.TransmissionType;
namespace Domain.Entities.Vehicles
{
    public sealed class TransmissionType : BaseEntity
    {
        public TransmissionTypeName Name { get; private set; } = null!;

        public ICollection<Vehicle> Vehicles { get; private set; } = [];

        private TransmissionType() { }

        public TransmissionType(TransmissionTypeName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(TransmissionTypeName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}