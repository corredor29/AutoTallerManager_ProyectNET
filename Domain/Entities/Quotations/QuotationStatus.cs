using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Quotations.QuotationStatus;
namespace Domain.Entities.Quotations
{

public sealed class QuotationStatus : BaseEntity
{
    public QuotationStatusName Name { get; private set; } = null!;

    public ICollection<Quotation> Quotations { get; private set; } = [];

    private QuotationStatus() { }

    public QuotationStatus(QuotationStatusName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void Update(QuotationStatusName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
}