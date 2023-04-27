namespace Prism.ProAssistant.Api.Controllers;

using Domain;
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

    public MaintenanceController(ILogger<MaintenanceController> logger, IQueryService queryService, IEventStore eventStore)
    {
        _logger = logger;
        _queryService = queryService;
        _eventStore = eventStore;
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