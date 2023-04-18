using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.Configuration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Controllers.Maintenance;

[Authorize]
public class RebuildEventController : Controller
{
    private readonly IQueryService _queryService;
    private readonly IEventService _eventService;
    private readonly ILogger<RebuildEventController> _logger;

    public RebuildEventController(ILogger<RebuildEventController> logger, IQueryService queryService, IEventService eventService)
    {
        _logger = logger;
        _queryService = queryService;
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

    private async Task RebuildEvents<T>() where T : IDataModel
    {
        _logger.LogInformation("Rebuilding events for {collection}", typeof(T).Name);
        var items = await _queryService.ListAsync<T>();

        foreach (var item in items)
        {
            await _eventService.AggregateAsync(item);
        }
    }
}