using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Domain;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration;
using Prism.ProAssistant.Domain.Configuration.DocumentConfiguration.Events;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.Configuration.Settings.Events;
using Prism.ProAssistant.Domain.Configuration.Tariffs;
using Prism.ProAssistant.Domain.Configuration.Tariffs.Events;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Maintenance;

[Authorize]
public class RebuildEventController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly ILogger<RebuildEventController> _logger;
    private readonly IQueryService _queryService;

    public RebuildEventController(IQueryService queryService, IEventStore eventStore, ILogger<RebuildEventController> logger)
    {
        _queryService = queryService;
        _eventStore = eventStore;
        _logger = logger;
    }

    [HttpPost]
    [Route("api/maintenance/rebuild-events")]
    public async Task Rebuild()
    {
        _logger.LogWarning("Rebuilding events for all collections");
        await RebuildEvents<Appointment>(item => new AppointmentCreated { Appointment = item });
        await RebuildEvents<Contact>(item => new ContactCreated
            { Contact = item });
        await RebuildEvents<DocumentConfiguration>(item => new DocumentConfigurationCreated
            { DocumentConfiguration = item });
        await RebuildEvents<Setting>(item => new SettingCreated
            { Setting = item });
        await RebuildEvents<Tariff>(item => new TariffCreated
            { Tariff = item });
    }

    private async Task RebuildEvents<T>(Func<T, IDomainEvent> factory)
    {
        _logger.LogInformation("Rebuilding events for {collection}", typeof(T).Name);
        var items = await _queryService.ListAsync<T>();

        foreach (var item in items)
        {
            await _eventStore.RaiseAndPersist<T>(factory(item));
        }
    }
}