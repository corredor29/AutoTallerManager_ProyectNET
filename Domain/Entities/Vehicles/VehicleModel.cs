using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Vehicles.VehicleModel;
namespace Domain.Entities.Vehicles
{

    public sealed class VehicleModel : BaseEntity
    {
        public int       BrandId   { get; private set; }
        public ModelName ModelName { get; private set; } = null!;

        public VehicleBrand          Brand    { get; private set; } = null!;
        public ICollection<Vehicle>  Vehicles { get; private set; } = [];

        private VehicleModel() { }

        public VehicleModel(int brandId, ModelName modelName)
        {
            BrandId   = brandId > 0 ? brandId : throw new ArgumentException("BrandId must be greater than 0.");
            ModelName = modelName ?? throw new ArgumentNullException(nameof(modelName));
        }

        public void Update(ModelName modelName)
        {
            ModelName = modelName ?? throw new ArgumentNullException(nameof(modelName));
        }
    }
}