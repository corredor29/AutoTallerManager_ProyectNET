using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Infrastructure.Context;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AutoTallerDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<DatabaseInitializer>();
        services.AddScoped<ICustomerRepository,            CustomerRepository>();
        services.AddScoped<IAppointmentRepository,         AppointmentRepository>();
        services.AddScoped<IVehicleRepository,             VehicleRepository>();
        services.AddScoped<IServiceTypeRepository,         ServiceTypeRepository>();
        services.AddScoped<IServiceOrderRepository,        ServiceOrderRepository>();
        services.AddScoped<IPartCategoryRepository,        PartCategoryRepository>();
        services.AddScoped<IMeasurementUnitRepository,     MeasurementUnitRepository>();
        services.AddScoped<IPartRepository,                PartRepository>();
        services.AddScoped<IServiceOrderPartRepository,    ServiceOrderPartRepository>();
        services.AddScoped<ISupplierRepository,            SupplierRepository>();
        services.AddScoped<IPartSupplierRepository,        PartSupplierRepository>();
        services.AddScoped<IPurchaseOrderStatusRepository, PurchaseOrderStatusRepository>();
        services.AddScoped<IPurchaseOrderRepository,       PurchaseOrderRepository>();
        services.AddScoped<IPurchaseOrderDetailRepository, PurchaseOrderDetailRepository>();
        services.AddScoped<IInvoiceRepository,             InvoiceRepository>();
        services.AddScoped<IInvoiceDetailRepository,       InvoiceDetailRepository>();
        services.AddScoped<IPaymentMethodRepository,       PaymentMethodRepository>();
        services.AddScoped<IPaymentRepository,             PaymentRepository>();
        services.AddScoped<IQuotationStatusRepository,     QuotationStatusRepository>();
        services.AddScoped<IQuotationRepository,           QuotationRepository>();
        services.AddScoped<IQuotationDetailRepository,     QuotationDetailRepository>();
        services.AddScoped<IUserRepository,                UserRepository>();
        services.AddScoped<IRoleRepository,                RoleRepository>();
        services.AddScoped<IUserRoleRepository,            UserRoleRepository>();
        services.AddScoped<ICustomerService,            CustomerService>();
        services.AddScoped<IAppointmentService,         AppointmentService>();
        services.AddScoped<IVehicleService,             VehicleService>();
        services.AddScoped<IServiceTypeService,         ServiceTypeService>();
        services.AddScoped<IServiceOrderService,        ServiceOrderService>();
        services.AddScoped<IPartCategoryService,        PartCategoryService>();
        services.AddScoped<IMeasurementUnitService,     MeasurementUnitService>();
        services.AddScoped<IPartService,                PartService>();
        services.AddScoped<IServiceOrderPartService,    ServiceOrderPartService>();
        services.AddScoped<ISupplierService,            SupplierService>();
        services.AddScoped<IPartSupplierService,        PartSupplierService>();
        services.AddScoped<IPurchaseOrderStatusService, PurchaseOrderStatusService>();
        services.AddScoped<IPurchaseOrderService,       PurchaseOrderService>();
        services.AddScoped<IPurchaseOrderDetailService, PurchaseOrderDetailService>();
        services.AddScoped<IInvoiceService,             InvoiceService>();
        services.AddScoped<IInvoiceDetailService,       InvoiceDetailService>();
        services.AddScoped<IPaymentMethodService,       PaymentMethodService>();
        services.AddScoped<IPaymentService,             PaymentService>();
        services.AddScoped<IQuotationStatusService,     QuotationStatusService>();
        services.AddScoped<IQuotationService,           QuotationService>();
        services.AddScoped<IQuotationDetailService,     QuotationDetailService>();
        services.AddScoped<IUserService,                UserService>();
        services.AddScoped<IAuthService,                AuthService>();
        services.AddScoped<IDocumentTypeRepository,   DocumentTypeRepository>();
        services.AddScoped<IEmailDomainRepository,    EmailDomainRepository>();
        services.AddScoped<IPhoneCodeRepository,      PhoneCodeRepository>();
        services.AddScoped<IPersonRepository,         PersonRepository>();
        services.AddScoped<IPersonDocumentRepository, PersonDocumentRepository>();
        services.AddScoped<IPersonEmailRepository,    PersonEmailRepository>();
        services.AddScoped<IPersonPhoneRepository,    PersonPhoneRepository>();
        services.AddScoped<IDocumentTypeService,   DocumentTypeService>();
        services.AddScoped<IEmailDomainService,    EmailDomainService>();
        services.AddScoped<IPhoneCodeService,      PhoneCodeService>();
        services.AddScoped<IPersonService,         PersonService>();
        services.AddScoped<IPersonDocumentService, PersonDocumentService>();
        services.AddScoped<IPersonEmailService,    PersonEmailService>();
        services.AddScoped<IPersonPhoneService,    PersonPhoneService>();
        services.AddScoped<IVehicleBrandRepository,            VehicleBrandRepository>();
        services.AddScoped<IVehicleModelRepository,            VehicleModelRepository>();
        services.AddScoped<IVehicleColorRepository,            VehicleColorRepository>();
        services.AddScoped<IFuelTypeRepository,                FuelTypeRepository>();
        services.AddScoped<ITransmissionTypeRepository,        TransmissionTypeRepository>();
        services.AddScoped<IVehicleOwnershipHistoryRepository, VehicleOwnershipHistoryRepository>();
        services.AddScoped<IMileageHistoryRepository,          MileageHistoryRepository>();
        services.AddScoped<IVehicleBrandService,            VehicleBrandService>();
        services.AddScoped<IVehicleModelService,            VehicleModelService>();
        services.AddScoped<IVehicleColorService,            VehicleColorService>();
        services.AddScoped<IFuelTypeService,                FuelTypeService>();
        services.AddScoped<ITransmissionTypeService,        TransmissionTypeService>();
        services.AddScoped<IVehicleOwnershipHistoryService, VehicleOwnershipHistoryService>();
        services.AddScoped<IMileageHistoryService,          MileageHistoryService>();
        services.AddScoped<IAppointmentStatusRepository, AppointmentStatusRepository>();
        services.AddScoped<IAppointmentStatusService,    AppointmentStatusService>();
        services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
        services.AddScoped<IOrderStatusService,    OrderStatusService>();
        services.AddScoped<IAuditActionTypeRepository, AuditActionTypeRepository>();
        services.AddScoped<IAuditLogRepository,        AuditLogRepository>();
        services.AddScoped<IAuditActionTypeService,    AuditActionTypeService>();
        services.AddScoped<IAuditLogService,           AuditLogService>();

        return services;
    }
}