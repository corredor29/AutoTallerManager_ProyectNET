using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Entities.Parts
{
    public class PartCategory : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Part> Parts { get; set; } = [];
    }
}