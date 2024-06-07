namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Domain.Configuration.Tariffs;
using Domain.Configuration.Tariffs.Events;
using Domain.DayToDay.Appointments;
using Helpers;
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
        ModelStateHelper.Validate(ModelState.IsValid);
        
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
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SearchAsync<Tariff>(request.ToArray());
    }

    [HttpGet]
    [Route("api/data/tariffs/{id}")]
    public async Task<Tariff?> Single(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SingleOrDefaultAsync<Tariff>(id);
    }

    [HttpPost]
    [Route("api/data/tariffs/update")]
    public async Task<UpsertResult> Update([FromBody] Tariff request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _eventStore.RaiseAndPersist<Tariff>(new TariffUpdated { Tariff = request });
    }
}