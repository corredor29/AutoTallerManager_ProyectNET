using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.ValueObject.Suppliers.Supplier;
namespace Domain.Entities.Suppliers
{

public sealed class Supplier : BaseEntity
{
    public CompanyName          CompanyName { get; private set; } = null!;
    public TaxId                TaxId       { get; private set; } = null!;
    public SupplierContactName  ContactName { get; private set; } = null!;
    public SupplierPhone        Phone       { get; private set; } = null!;
    public SupplierEmail        Email       { get; private set; } = null!;
    public SupplierAddress      Address     { get; private set; } = null!;
    public bool                 IsActive    { get; private set; }

    public ICollection<PartSupplier>  PartSuppliers  { get; private set; } = [];
    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = [];

    private Supplier() { }

    public Supplier(CompanyName companyName, TaxId taxId, SupplierContactName contactName,
                    SupplierPhone phone, SupplierEmail email, SupplierAddress address)
    {
        CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        TaxId       = taxId       ?? throw new ArgumentNullException(nameof(taxId));
        ContactName = contactName ?? throw new ArgumentNullException(nameof(contactName));
        Phone       = phone       ?? throw new ArgumentNullException(nameof(phone));
        Email       = email       ?? throw new ArgumentNullException(nameof(email));
        Address     = address     ?? throw new ArgumentNullException(nameof(address));
        IsActive    = true;
    }

    public void Update(CompanyName companyName, TaxId taxId, SupplierContactName contactName,
                       SupplierPhone phone, SupplierEmail email, SupplierAddress address)
    {
        CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
        TaxId       = taxId       ?? throw new ArgumentNullException(nameof(taxId));
        ContactName = contactName ?? throw new ArgumentNullException(nameof(contactName));
        Phone       = phone       ?? throw new ArgumentNullException(nameof(phone));
        Email       = email       ?? throw new ArgumentNullException(nameof(email));
        Address     = address     ?? throw new ArgumentNullException(nameof(address));
    }

    public void Activate()   => IsActive = true;
    public void Deactivate() => IsActive = false;
}
}