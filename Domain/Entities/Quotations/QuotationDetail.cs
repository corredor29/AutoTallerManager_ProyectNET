using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Parts;
using Domain.ValueObject.Quotations.QuotationDetail;
namespace Domain.Entities.Quotations
{
public sealed class QuotationDetail : BaseEntity
    {
        public int                QuotationId { get; private set; }
        public int                PartId      { get; private set; }
        public QuotationQuantity  Quantity    { get; private set; } = null!;
        public QuotationUnitPrice UnitPrice   { get; private set; } = null!;

        public Quotation Quotation { get; private set; } = null!;
        public Part      Part      { get; private set; } = null!;

        private QuotationDetail() { }

        public QuotationDetail(int quotationId, int partId,
                            QuotationQuantity quantity, QuotationUnitPrice unitPrice)
        {
            QuotationId = quotationId > 0 ? quotationId : throw new ArgumentException("QuotationId must be greater than 0.");
            PartId      = partId      > 0 ? partId      : throw new ArgumentException("PartId must be greater than 0.");
            Quantity    = quantity  ?? throw new ArgumentNullException(nameof(quantity));
            UnitPrice   = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        }

        public void Update(QuotationQuantity quantity, QuotationUnitPrice unitPrice)
        {
            Quantity  = quantity  ?? throw new ArgumentNullException(nameof(quantity));
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        }
    }
}