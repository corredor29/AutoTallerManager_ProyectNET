using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Domain.Common;
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
        // Permite leer el usuario autenticado actual para registrar auditoria.
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Evita que el proceso de auditoria se dispare recursivamente al guardar los logs.
        private bool _isAuditing;

        public AutoTallerDbContext(
            DbContextOptions<AutoTallerDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        // Cada DbSet representa una tabla o agregado principal del dominio.
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

            // Aplica automaticamente todas las configuraciones EntityTypeConfiguration del ensamblado.
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AutoTallerDbContext).Assembly
            );
        }

        public override int SaveChanges()
        {
            // Redirige al flujo asincrono para reutilizar la misma logica de auditoria.
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_isAuditing)
            {
                // Cuando ya se esta auditando, guarda directo para no entrar en bucle.
                return await base.SaveChangesAsync(cancellationToken);
            }

            // Captura los cambios relevantes antes de persistirlos para luego generar su log.
            var pendingAuditEntries = PrepareAuditEntries();

            _isAuditing = true;

            try
            {
                var result = await base.SaveChangesAsync(cancellationToken);

                if (pendingAuditEntries.Count > 0)
                {
                    // Despues del guardado principal ya existe el id final y se puede registrar auditoria.
                    await PersistAuditEntriesAsync(pendingAuditEntries, cancellationToken);
                }

                return result;
            }
            finally
            {
                _isAuditing = false;
            }
        }

        private List<PendingAuditEntry> PrepareAuditEntries()
        {
            var pendingAuditEntries = new List<PendingAuditEntry>();

            // Recorre solo entidades del dominio para detectar altas, cambios y eliminaciones.
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                {
                    continue;
                }

                // Los propios logs de auditoria no deben auditarse a si mismos.
                if (entry.Entity is AuditLog or AuditActionType)
                {
                    continue;
                }

                pendingAuditEntries.Add(new PendingAuditEntry
                {
                    Entry = entry,
                    EntityName = entry.Metadata.ClrType.Name,
                    ActionName = entry.State switch
                    {
                        EntityState.Added => "Create",
                        EntityState.Modified => "Update",
                        EntityState.Deleted => "Delete",
                        _ => string.Empty
                    },
                    RecordId = GetRecordId(entry),
                    Description = BuildDescription(entry)
                });
            }

            return pendingAuditEntries;
        }

        private async Task PersistAuditEntriesAsync(
            IReadOnlyCollection<PendingAuditEntry> pendingAuditEntries,
            CancellationToken cancellationToken)
        {
            // Usa el usuario autenticado actual o, si no existe, un usuario base del sistema.
            var currentUserId = ResolveCurrentUserId() ?? await ResolveFallbackUserIdAsync(cancellationToken);
            if (!currentUserId.HasValue)
            {
                return;
            }

            // Resuelve los ids de tipos de accion ya sembrados en catalogo.
            var actionNames = pendingAuditEntries
                .Select(x => x.ActionName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var actionTypes = (await AuditActionTypes
                .ToListAsync(cancellationToken))
                .Where(x => actionNames.Contains(x.Name.Value))
                .ToDictionary(x => x.Name.Value, x => x.Id);

            if (actionTypes.Count == 0)
            {
                return;
            }

            foreach (var pendingAuditEntry in pendingAuditEntries)
            {
                if (!actionTypes.TryGetValue(pendingAuditEntry.ActionName, out var actionTypeId))
                {
                    continue;
                }

                // En inserts el id final puede quedar disponible solo despues del SaveChanges principal.
                if (pendingAuditEntry.RecordId <= 0)
                {
                    pendingAuditEntry.RecordId = GetRecordId(pendingAuditEntry.Entry);
                }

                if (pendingAuditEntry.RecordId <= 0)
                {
                    continue;
                }

                AuditLogs.Add(new AuditLog(
                    currentUserId.Value,
                    actionTypeId,
                    new Domain.ValueObject.Audit.AuditLog.AffectedEntityName(pendingAuditEntry.EntityName),
                    pendingAuditEntry.RecordId,
                    new Domain.ValueObject.Audit.AuditLog.AuditDescription(pendingAuditEntry.Description)));
            }

            if (ChangeTracker.Entries<AuditLog>().Any(x => x.State == EntityState.Added))
            {
                // Guarda un segundo ciclo solo si efectivamente se agregaron logs.
                await base.SaveChangesAsync(cancellationToken);
            }
        }

        private int GetRecordId(EntityEntry entry)
        {
            // Intenta leer la PK actual y, si no existe aun, la original.
            var primaryKey = entry.Properties.FirstOrDefault(x => x.Metadata.IsPrimaryKey());
            if (primaryKey is null)
            {
                return 0;
            }

            if (primaryKey.CurrentValue is int currentId && currentId > 0)
            {
                return currentId;
            }

            if (primaryKey.OriginalValue is int originalId && originalId > 0)
            {
                return originalId;
            }

            return 0;
        }

        private static string BuildDescription(EntityEntry entry)
        {
            // Genera un mensaje simple y entendible segun el tipo de operacion detectada.
            return entry.State switch
            {
                EntityState.Added => $"Created {entry.Metadata.ClrType.Name}.",
                EntityState.Deleted => $"Deleted {entry.Metadata.ClrType.Name}.",
                EntityState.Modified => BuildUpdateDescription(entry),
                _ => $"{entry.Metadata.ClrType.Name} changed."
            };
        }

        private static string BuildUpdateDescription(EntityEntry entry)
        {
            // En updates lista las propiedades modificadas para que el log sea mas util.
            var modifiedProperties = entry.Properties
                .Where(x => x.IsModified && !x.Metadata.IsPrimaryKey())
                .Select(x => x.Metadata.Name)
                .ToArray();

            return modifiedProperties.Length == 0
                ? $"Updated {entry.Metadata.ClrType.Name}."
                : $"Updated {entry.Metadata.ClrType.Name}: {string.Join(", ", modifiedProperties)}.";
        }

        private int? ResolveCurrentUserId()
        {
            // Busca el id del usuario en los claims mas comunes del token JWT.
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var claimValue = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

            return int.TryParse(claimValue, out var userId) && userId > 0
                ? userId
                : null;
        }

        private async Task<int?> ResolveFallbackUserIdAsync(CancellationToken cancellationToken)
        {
            // Como respaldo usa el primer usuario disponible para no perder auditoria del sistema.
            return await Users
                .OrderBy(x => x.Id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private sealed class PendingAuditEntry
        {
            // Guarda temporalmente la informacion necesaria antes de crear el AuditLog definitivo.
            public required EntityEntry Entry { get; init; }
            public required string EntityName { get; init; }
            public required string ActionName { get; init; }
            public required string Description { get; init; }
            public int RecordId { get; set; }
        }
    }
}
