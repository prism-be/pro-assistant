namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Domain.Configuration.Tariffs;
using Domain.Configuration.Tariffs.Events;
using Domain.DayToDay.Appointments;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Storage.Events;

[Authorize]
public class TariffController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public TariffController(IQueryService queryService, IEventStore eventStore)
    {
        _queryService = queryService;
        _eventStore = eventStore;
    }

    [HttpPost]
    [Route("api/data/tariffs/insert")]
    public async Task<UpsertResult> Insert([FromBody] Tariff request)
    {
        request.Id = Identifier.GenerateString();
        return await _eventStore.RaiseAndPersist<Tariff>(new TariffCreated { Tariff = request });
    }

    [HttpGet]
    [Route("api/data/tariffs")]
    public async Task<IEnumerable<Tariff>> List()
    {
        return await _queryService.ListAsync<Tariff>();
    }

    [HttpPost]
    [Route("api/data/tariffs/search")]
    public async Task<IEnumerable<Tariff>> Search([FromBody] IEnumerable<Filter> request)
    {
        return await _queryService.SearchAsync<Tariff>(request.ToArray());
    }

    [HttpGet]
    [Route("api/data/tariffs/{id}")]
    public async Task<Tariff?> Single(string id)
    {
        return await _queryService.SingleOrDefaultAsync<Tariff>(id);
    }

    [HttpPost]
    [Route("api/data/tariffs/update")]
    public async Task<UpsertResult> Update([FromBody] Tariff request)
    {
        var previous = await _queryService.SingleOrDefaultAsync<Tariff>(request.Id);
        var updated = await _eventStore.RaiseAndPersist<Tariff>(new TariffUpdated { Tariff = request });

        if (previous == null)
        {
            return updated;
        }

        var filter = new Filter(nameof(Appointment.TypeId), request.Id);
        var appointments = await _queryService.SearchAsync<Appointment>(filter);

        foreach (var appointment in appointments)
        {
            await _eventStore.Persist<Appointment>(appointment.Id);
        }

        return updated;
    }
}