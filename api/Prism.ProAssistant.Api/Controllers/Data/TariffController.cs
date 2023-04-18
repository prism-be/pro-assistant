using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain.Configuration.Tariffs;
using Prism.ProAssistant.Domain.Configuration.Tariffs.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Data;

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
        return await _queryService.SearchAsync<Tariff>(request);
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
        var updated = await _eventStore.RaiseAndPersist<Tariff>(new TariffUpdated { Tariff = request });

        // TODO
        // var filter = new FieldValue(nameof(Appointment.TypeId), request.Id);
        //
        // await _eventStore.UpdateManyAsync<Appointment>(filter,
        //     new FieldValue(nameof(Appointment.ForeColor), request.ForeColor),
        //     new FieldValue(nameof(Appointment.BackgroundColor), request.BackgroundColor)
        // );

        return updated;
    }
}