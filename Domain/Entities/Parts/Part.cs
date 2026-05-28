using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Quotations;
using Domain.Entities.Suppliers;
using Domain.ValueObject.Parts.Part;

namespace Domain.Entities.Parts
{

    public sealed class Part : BaseEntity
    {
        public int          PartCategoryId { get; private set; }
        public int?         UnitId         { get; private set; }
        public PartCode        Code        { get; private set; } = null!;
        public PartDescription Description { get; private set; } = null!;
        public PartStock       Stock       { get; private set; } = null!;
        public PartMinStock    MinStock    { get; private set; } = null!;
        public PartUnitPrice   UnitPrice   { get; private set; } = null!;
        public bool            IsActive    { get; private set; }

        public PartCategory                  Category         { get; private set; } = null!;
        public MeasurementUnit?              Unit             { get; private set; }
        public ICollection<ServiceOrderPart> OrderParts       { get; private set; } = [];
        public ICollection<PartSupplier>     PartSuppliers    { get; private set; } = [];
        public ICollection<QuotationDetail>  QuotationDetails { get; private set; } = [];

        private Part() { }

        public Part(int partCategoryId, PartCode code, PartDescription description,
                    PartStock stock, PartMinStock minStock, PartUnitPrice unitPrice,
                    int? unitId = null)
        {
            PartCategoryId = partCategoryId > 0 ? partCategoryId : throw new ArgumentException("PartCategoryId must be greater than 0.");
            Code           = code        ?? throw new ArgumentNullException(nameof(code));
            Description    = description ?? throw new ArgumentNullException(nameof(description));
            Stock          = stock       ?? throw new ArgumentNullException(nameof(stock));
            MinStock       = minStock    ?? throw new ArgumentNullException(nameof(minStock));
            UnitPrice      = unitPrice   ?? throw new ArgumentNullException(nameof(unitPrice));
            UnitId         = unitId;
            IsActive       = true;
        }

        public void Update(PartDescription description, PartUnitPrice unitPrice, int? unitId)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            UnitPrice   = unitPrice   ?? throw new ArgumentNullException(nameof(unitPrice));
            UnitId      = unitId;
        }

        public void AddStock(PartStock quantity)
        {
            if (quantity is null) throw new ArgumentNullException(nameof(quantity));
            Stock = new PartStock(Stock.Value + quantity.Value);
        }

        public void RemoveStock(PartStock quantity)
        {
            if (quantity is null) throw new ArgumentNullException(nameof(quantity));
            if (quantity.Value > Stock.Value)
                throw new InvalidOperationException("Insufficient stock.");
            Stock = new PartStock(Stock.Value - quantity.Value);
        }

        public bool HasSufficientStock(int quantity) => Stock.Value >= quantity;
        public bool IsBelowMinStock() => Stock.Value < MinStock.Value;

        public void Activate()   => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}