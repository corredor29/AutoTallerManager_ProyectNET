using Application.Requests.Quotations;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Tests.Infrastructure;

public sealed class QuotationServiceTests
{
    private static QuotationService CreateService(AutoTallerDbContext db) =>
        new(new QuotationRepository(db), db);

    private static async Task<(int quotationId, int serviceOrderId, int rejectedStatusId)>
        SeedScenarioAsync(AutoTallerDbContext db, decimal laborCost = 150m)
    {
        var statuses         = await SeedDataHelper.SeedOrderStatusesAsync(db);
        var serviceTypes     = await SeedDataHelper.SeedServiceTypesAsync(db);
        var (_, modelId)     = await SeedDataHelper.SeedVehicleModelAsync(db);
        var vehicleId        = await SeedDataHelper.SeedVehicleAsync(db, modelId);
        var (_, mechanicId)  = await SeedDataHelper.SeedMechanicAsync(db);
        var quotationStatuses = await SeedDataHelper.SeedQuotationStatusesAsync(db);

        var serviceOrderId = await SeedDataHelper.SeedServiceOrderAsync(
            db, vehicleId, serviceTypes.DiagnosticsId, mechanicId, statuses.PendingId);

        var quotationId = await SeedDataHelper.SeedQuotationAsync(
            db, serviceOrderId, mechanicId, quotationStatuses.PendingId, laborCost);

        return (quotationId, serviceOrderId, quotationStatuses.RejectedId);
    }

    // ── ChangeStatusAsync → Accept ───────────────────────────────────

    [Fact]
    public async Task ChangeStatusAsync_Accept_ChangesStatusToAccepted()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        var (quotationId, _, _)    = await SeedScenarioAsync(db);
        var statuses               = await db.QuotationStatuses.ToListAsync();
        var acceptedId             = statuses.First(s => s.Name.Value == "Accepted").Id;

        var result = await service.ChangeStatusAsync(
            quotationId,
            new ChangeQuotationStatusRequest { QuotationStatusId = acceptedId });

        result.Should().BeTrue();

        var saved = await db.Quotations.FindAsync(quotationId);
        saved!.QuotationStatusId.Should().Be(acceptedId);
        saved.RespondedAt.Should().NotBeNull();
    }

    // ── ChangeStatusAsync → Reject ───────────────────────────────────

    [Fact]
    public async Task ChangeStatusAsync_Reject_ChangesStatusAndCreatesDiagnosisOnlyInvoice()
    {
        var db                             = DbContextFactory.Create();
        var service                        = CreateService(db);
        var (quotationId, serviceOrderId, rejectedId) = await SeedScenarioAsync(db, laborCost: 200m);

        var result = await service.ChangeStatusAsync(
            quotationId,
            new ChangeQuotationStatusRequest
            {
                QuotationStatusId = rejectedId,
                RejectionReason   = "Client rejected the estimate."
            });

        result.Should().BeTrue();

        var invoice = await db.Invoices
            .FirstOrDefaultAsync(i => i.ServiceOrderId == serviceOrderId);

        invoice.Should().NotBeNull();
        invoice!.DiagnosisOnlyCharged.Should().BeTrue();
        invoice.Total.Value.Should().Be(200m);
        invoice.Tax.Value.Should().Be(0m);
        invoice.QuotationId.Should().Be(quotationId);
    }

    [Fact]
    public async Task ChangeStatusAsync_Reject_ServiceOrderAlreadyHasInvoice_DoesNotDuplicateInvoice()
    {
        var db                             = DbContextFactory.Create();
        var service                        = CreateService(db);
        var (quotationId, serviceOrderId, rejectedId) = await SeedScenarioAsync(db);

        // Reject once — creates the invoice
        await service.ChangeStatusAsync(
            quotationId,
            new ChangeQuotationStatusRequest { QuotationStatusId = rejectedId });

        // Change back to Pending to allow a second rejection attempt
        var statuses  = await db.QuotationStatuses.ToListAsync();
        var pendingId = statuses.First(s => s.Name.Value == "Pending").Id;

        var secondQuotationId = await SeedDataHelper.SeedQuotationAsync(
            db, serviceOrderId,
            (await db.Users.FirstAsync()).Id,
            pendingId);

        // Reject the second quotation — should NOT create another invoice
        await service.ChangeStatusAsync(
            secondQuotationId,
            new ChangeQuotationStatusRequest { QuotationStatusId = rejectedId });

        var invoiceCount = await db.Invoices
            .CountAsync(i => i.ServiceOrderId == serviceOrderId);

        invoiceCount.Should().Be(1);
    }

    [Fact]
    public async Task ChangeStatusAsync_QuotationNotFound_ReturnsFalse()
    {
        var db      = DbContextFactory.Create();
        var service = CreateService(db);
        await SeedDataHelper.SeedQuotationStatusesAsync(db);
        var statuses    = await db.QuotationStatuses.ToListAsync();
        var acceptedId  = statuses.First(s => s.Name.Value == "Accepted").Id;

        var result = await service.ChangeStatusAsync(
            9999,
            new ChangeQuotationStatusRequest { QuotationStatusId = acceptedId });

        result.Should().BeFalse();
    }
}
