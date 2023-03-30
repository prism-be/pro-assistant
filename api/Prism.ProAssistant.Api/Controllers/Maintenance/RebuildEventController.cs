using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Maintenance;

[Authorize]
public class RebuildEventController: Controller
{
    private readonly ILogger<RebuildEventController> _logger;
    private readonly IDataService _dataService;
    private readonly IEventService _eventService;

    public RebuildEventController(ILogger<RebuildEventController> logger, IDataService dataService, IEventService eventService)
    {
        _logger = logger;
        _dataService = dataService;
        _eventService = eventService;
    }

    [HttpPost]
    [Route("api/maintenance/rebuild-events")]
    public async Task Rebuild()
    {
        _logger.LogWarning("Rebuilding events for all collections");
        await RebuildEvents<Appointment>();
        await RebuildEvents<Contact>();
        await RebuildEvents<DocumentConfiguration>();
        await RebuildEvents<Setting>();
        await RebuildEvents<Tariff>();
    }

    private async Task RebuildEvents<T>() where T: IDataModel
    {
        _logger.LogInformation("Rebuilding events for {collection}", typeof(T).Name);
        var items = await _dataService.ListAsync<T>();
        foreach (var item in items)
        {
            await _eventService.AggregateAsync(item);
        }
    }
}