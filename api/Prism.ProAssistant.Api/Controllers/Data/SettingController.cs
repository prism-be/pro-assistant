using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain.Configuration.Settings;
using Prism.ProAssistant.Domain.Configuration.Settings.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class SettingController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public SettingController(IQueryService queryService, IEventStore eventStore)
    {
        _queryService = queryService;
        _eventStore = eventStore;
    }

    [HttpPost]
    [Route("api/data/settings/insert")]
    public async Task<UpsertResult> Insert([FromBody] Setting request)
    {
        return await _eventStore.RaiseAndPersist<Setting>(new SettingCreated { Setting = request });
    }

    [HttpGet]
    [Route("api/data/settings")]
    public async Task<IEnumerable<Setting>> List()
    {
        return await _queryService.ListAsync<Setting>();
    }

    [HttpPost]
    [Route("api/data/settings/search")]
    public async Task<IEnumerable<Setting>> Search([FromBody] IEnumerable<Filter> request)
    {
        return await _queryService.SearchAsync<Setting>(request.ToArray());
    }

    [HttpGet]
    [Route("api/data/settings/{id}")]
    public async Task<Setting?> Single(string id)
    {
        return await _queryService.SingleOrDefaultAsync<Setting>(id);
    }

    [HttpPost]
    [Route("api/data/settings/update")]
    public async Task<UpsertResult> Update([FromBody] Setting request)
    {
        return await _eventStore.RaiseAndPersist<Setting>(new SettingUpdated { Setting = request });
    }

    [HttpPost]
    [Route("api/data/settings/update-many")]
    public async Task<List<UpsertResult>> UpdateMany([FromBody] List<Setting> request)
    {
        var results = new List<UpsertResult>();

        foreach (var setting in request)
        {
            results.Add(await _eventStore.RaiseAndPersist<Setting>(new SettingUpdated { Setting = setting }));
        }

        return results;
    }
}