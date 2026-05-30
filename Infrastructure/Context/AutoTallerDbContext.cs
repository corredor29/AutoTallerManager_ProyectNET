using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Persons;
using Domain.Entities.Customers;
using Domain.Entities.Users;
using Domain.Entities.Vehicles;
using Domain.Entities.Appointments;
using Domain.Entities.ServiceOrders;
using Domain.Entities.Parts;
using Domain.Entities.Invoices;
using Domain.Entities.Audit;
using Domain.Entities.Quotations;
using Domain.Entities.Suppliers;

namespace Infrastructure.Context
{

    public class AutoTallerDbContext : DbContext
    {
        public AutoTallerDbContext(DbContextOptions<AutoTallerDbContext> options)
            : base(options) { }
        public DbSet<Person>         Persons         => Set<Person>();
        public DbSet<DocumentType>   DocumentTypes   => Set<DocumentType>();
        public DbSet<PersonDocument> PersonDocuments => Set<PersonDocument>();
        public DbSet<EmailDomain>    EmailDomains    => Set<EmailDomain>();
        public DbSet<PersonEmail>    PersonEmails    => Set<PersonEmail>();
        public DbSet<PhoneCode>      PhoneCodes      => Set<PhoneCode>();
        public DbSet<PersonPhone>    PersonPhones    => Set<PersonPhone>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<User>     Users     => Set<User>();
        public DbSet<Role>     Roles     => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<VehicleBrand>            VehicleBrands            => Set<VehicleBrand>();
        public DbSet<VehicleModel>            VehicleModels            => Set<VehicleModel>();
        public DbSet<VehicleColor>            VehicleColors            => Set<VehicleColor>();
        public DbSet<FuelType>                FuelTypes                => Set<FuelType>();
        public DbSet<TransmissionType>        TransmissionTypes        => Set<TransmissionType>();
        public DbSet<Vehicle>                 Vehicles                 => Set<Vehicle>();
        public DbSet<VehicleOwnershipHistory> VehicleOwnershipHistories => Set<VehicleOwnershipHistory>();
        public DbSet<MileageHistory>          MileageHistories         => Set<MileageHistory>();
        public DbSet<AppointmentStatus> AppointmentStatuses => Set<AppointmentStatus>();
        public DbSet<Appointment>       Appointments        => Set<Appointment>();
        public DbSet<ServiceType>  ServiceTypes  => Set<ServiceType>();
        public DbSet<OrderStatus>  OrderStatuses => Set<OrderStatus>();
        public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
        public DbSet<PartCategory>    PartCategories    => Set<PartCategory>();
        public DbSet<MeasurementUnit> MeasurementUnits  => Set<MeasurementUnit>();
        public DbSet<Part>            Parts             => Set<Part>();
        public DbSet<ServiceOrderPart> ServiceOrderParts => Set<ServiceOrderPart>();
        public DbSet<QuotationStatus> QuotationStatuses => Set<QuotationStatus>();
        public DbSet<Quotation>       Quotations        => Set<Quotation>();
        public DbSet<QuotationDetail> QuotationDetails  => Set<QuotationDetail>();
        public DbSet<Supplier>            Suppliers            => Set<Supplier>();
        public DbSet<PartSupplier>        PartSuppliers        => Set<PartSupplier>();
        public DbSet<PurchaseOrderStatus> PurchaseOrderStatuses => Set<PurchaseOrderStatus>();
        public DbSet<PurchaseOrder>       PurchaseOrders       => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
        public DbSet<Invoice>       Invoices       => Set<Invoice>();
        public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<Payment>       Payments       => Set<Payment>();
        public DbSet<AuditActionType> AuditActionTypes => Set<AuditActionType>();
        public DbSet<AuditLog>        AuditLogs        => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<PersonDocument>();
            modelBuilder.Ignore<DocumentType>();
            modelBuilder.Ignore<PhoneCode>();
            modelBuilder.Ignore<PersonPhone>();
            modelBuilder.Ignore<VehicleOwnershipHistory>();
            modelBuilder.Ignore<MileageHistory>();
            modelBuilder.Ignore<AuditActionType>();
            modelBuilder.Ignore<AuditLog>();

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AutoTallerDbContext).Assembly
            );
        }
    }
}
