using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class SettingController : Controller
{
    private readonly IDataService _dataService;

    public SettingController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpPost]
    [Route("api/data/settings/insert")]
    public async Task<UpsertResult> Insert([FromBody] Setting request)
    {
        return await _dataService.InsertAsync(request);
    }

    [HttpGet]
    [Route("api/data/settings")]
    public async Task<List<Setting>> List()
    {
        return await _dataService.ListAsync<Setting>();
    }

    [HttpPost]
    [Route("api/data/settings/search")]
    public async Task<List<Setting>> Search([FromBody] List<SearchFilter> request)
    {
        return await _dataService.SearchAsync<Setting>(request);
    }

    [HttpGet]
    [Route("api/data/settings/{id}")]
    public async Task<Setting?> Single(string id)
    {
        return await _dataService.SingleOrDefaultAsync<Setting>(id);
    }

    [HttpPost]
    [Route("api/data/settings/update")]
    public async Task<UpsertResult> Update([FromBody] Setting request)
    {
        return await _dataService.ReplaceAsync(request);
    }

    [HttpPost]
    [Route("api/data/settings/update-many")]
    public async Task<List<UpsertResult>> UpdateMany([FromBody] List<Setting> request)
    {
        return await _dataService.ReplaceManyAsync(request);
    }
}