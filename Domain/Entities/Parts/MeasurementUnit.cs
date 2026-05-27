using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Parts
{
    public class MeasurementUnit : BaseEntity
    {
        public string Name         { get; set; } = null!;
        public string Abbreviation { get; set; } = null!;

        public ICollection<Part> Parts { get; set; } = [];
    }
}