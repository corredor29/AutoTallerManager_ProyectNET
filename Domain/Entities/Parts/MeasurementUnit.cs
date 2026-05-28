using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Parts.MeasurementUnit;
namespace Domain.Entities.Parts
{
    public sealed class MeasurementUnit : BaseEntity
    {
        public MeasurementUnitName         Name         { get; private set; } = null!;
        public MeasurementUnitAbbreviation Abbreviation { get; private set; } = null!;

        public ICollection<Part> Parts { get; private set; } = [];

        private MeasurementUnit() { }

        public MeasurementUnit(MeasurementUnitName name, MeasurementUnitAbbreviation abbreviation)
        {
            Name         = name         ?? throw new ArgumentNullException(nameof(name));
            Abbreviation = abbreviation ?? throw new ArgumentNullException(nameof(abbreviation));
        }

        public void Update(MeasurementUnitName name, MeasurementUnitAbbreviation abbreviation)
        {
            Name         = name         ?? throw new ArgumentNullException(nameof(name));
            Abbreviation = abbreviation ?? throw new ArgumentNullException(nameof(abbreviation));
        }
    }
}