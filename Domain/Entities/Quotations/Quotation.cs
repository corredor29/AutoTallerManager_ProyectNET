using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities.ServiceOrders;
using Domain.Entities.Users;
using Domain.Entities.Invoices;
namespace Domain.Entities.Quotations
{
    public class Quotation : BaseEntity
    {
        public int       ServiceOrderId    { get; set; }
        public int       CreatedByUserId   { get; set; }
        public int       QuotationStatusId { get; set; }
        public DateTime  CreatedAt         { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt       { get; set; }
        public decimal   LaborCost         { get; set; } = 0;
        public decimal   Subtotal          { get; set; } = 0;
        public decimal   Total             { get; set; } = 0;
        public string?   RejectionReason   { get; set; }
        public string?   Notes             { get; set; }

        public ServiceOrder               ServiceOrder    { get; set; } = null!;
        public User                       CreatedByUser   { get; set; } = null!;
        public QuotationStatus            QuotationStatus { get; set; } = null!;
        public ICollection<QuotationDetail> Details       { get; set; } = default!;
        public Invoice?                   Invoice         { get; set; }
    }
}