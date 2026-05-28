using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Parts.PartCategory;
namespace Domain.Entities.Parts
{
    public sealed class PartCategory : BaseEntity
    {
        public PartCategoryName Name { get; private set; } = null!;

        public ICollection<Part> Parts { get; private set; } = [];

        private PartCategory() { }

        public PartCategory(PartCategoryName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void Update(PartCategoryName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}