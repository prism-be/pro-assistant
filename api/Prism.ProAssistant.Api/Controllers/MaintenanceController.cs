namespace Prism.ProAssistant.Api.Controllers;

using Domain;
using Domain.Accounting.Reporting;
using Domain.Configuration.DocumentConfiguration;
using Domain.Configuration.Settings;
using Domain.Configuration.Tariffs;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Storage.Events;

[Authorize]
public class MaintenanceController: Controller
{
    private readonly ILogger<MaintenanceController> _logger;
    private readonly IQueryService _queryService;
    private readonly IEventStore _eventStore;
    private readonly IStateProvider _stateProvider;

    public MaintenanceController(ILogger<MaintenanceController> logger, IQueryService queryService, IEventStore eventStore, IStateProvider stateProvider)
    {
        _logger = logger;
        _queryService = queryService;
        _eventStore = eventStore;
        _stateProvider = stateProvider;
    }

    [HttpPost]
    [Route("api/maintenance/rebuild")]
    public async Task RebuildProjections()
    {
        await RebuildProjections<Appointment>("appointments");
        await RebuildProjections<Contact>("contacts");
        
        await RebuildProjections<DocumentConfiguration>("documents-configuration");
        await RebuildProjections<Setting>("settings");
        await RebuildProjections<Tariff>("tariffs");
    }
    
    [HttpPost]
    [Route("api/maintenance/rebuild-accounting-periods")]
    public async Task RebuildAccountingPeriods()
    {
        var container = await _stateProvider.GetContainerAsync<AccountingReportingPeriod>();
        
        var appointments = (await _queryService.ListAsync<Appointment>()).ToList();
        
        var start = appointments.MinBy(x => x.StartDate)?.StartDate ?? DateTime.Now;
        var end = appointments.MaxBy(x => x.StartDate)?.StartDate ?? DateTime.Now;
        
        var startPeriod = new DateTime(start.Year, start.Month, 1);

        while (startPeriod <= end)
        {
            var endPeriod = startPeriod.AddMonths(1).AddDays(-1);
            var periodAppointments = appointments.Where(x => x.StartDate >= startPeriod && x.StartDate <= endPeriod).ToList();
            
            var accountingPeriod = AccountingReportingPeriodProjection.Project(12, periodAppointments);
            await container.WriteAsync(accountingPeriod.Id, accountingPeriod);
            
            startPeriod = startPeriod.AddMonths(1);
        }
    }
    
    private async Task RebuildProjections<T>(string streamType)
    {
        _logger.LogInformation("Rebuilding projections for {Name}", typeof(T).Name);
        var streams = await _queryService.DistinctAsync<DomainEvent, string>(nameof(DomainEvent.StreamId), new Filter(nameof(DomainEvent.StreamType), streamType));

        foreach (var stream in streams)
        {
            _logger.LogInformation("Rebuilding projection for {Name} - {StreamId}", typeof(T).Name, stream);
            await _eventStore.Persist<T>(stream);
        }
    }
}