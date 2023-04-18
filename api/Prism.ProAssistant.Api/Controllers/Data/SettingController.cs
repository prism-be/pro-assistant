using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class SettingController : Controller, IDataController<Setting>
{
    private readonly IQueryService _queryService;
    private readonly IEventService _eventService;

    public SettingController(IQueryService queryService, IEventService eventService)
    {
        _queryService = queryService;
        _eventService = eventService;
    }

    [HttpPost]
    [Route("api/data/settings/insert")]
    public async Task<UpsertResult> Insert([FromBody] Setting request)
    {
        return await _eventService.CreateAsync(request);
    }

    [HttpGet]
    [Route("api/data/settings")]
    public async Task<List<Setting>> List()
    {
        return await _queryService.ListAsync<Setting>();
    }

    [HttpPost]
    [Route("api/data/settings/search")]
    public async Task<List<Setting>> Search([FromBody] List<SearchFilter> request)
    {
        return await _queryService.SearchAsync<Setting>(request);
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
        return await _eventService.ReplaceAsync(request);
    }

    [HttpPost]
    [Route("api/data/settings/update-many")]
    public async Task<List<UpsertResult>> UpdateMany([FromBody] List<Setting> request)
    {
        return await _eventService.ReplaceManyAsync(request);
    }
}