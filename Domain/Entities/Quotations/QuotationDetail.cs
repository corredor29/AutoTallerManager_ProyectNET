using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.Parts;
namespace Domain.Entities.Quotations
{
    public class QuotationDetail : BaseEntity
    {
        public int     QuotationId { get; set; }
        public int     PartId      { get; set; }
        public int     Quantity    { get; set; }
        public decimal UnitPrice   { get; set; }

        public Quotation Quotation { get; set; } = null!;
        public Part      Part      { get; set; } = null!;
    }
}