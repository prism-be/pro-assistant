using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class TariffController : Controller
{
    private readonly IDataService _dataService;

    public TariffController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpPost]
    [Route("api/data/tariffs/insert")]
    public async Task<UpsertResult> Insert([FromBody] Tariff request)
    {
        return await _dataService.InsertAsync(request);
    }

    [HttpGet]
    [Route("api/data/tariffs")]
    public async Task<List<Tariff>> List()
    {
        return await _dataService.ListAsync<Tariff>();
    }

    [HttpPost]
    [Route("api/data/tariffs/search")]
    public async Task<List<Tariff>> Search([FromBody] List<SearchFilter> request)
    {
        return await _dataService.SearchAsync<Tariff>(request);
    }

    [HttpPost]
    [Route("api/data/tariffs/update")]
    public async Task<UpsertResult> Search([FromBody] Tariff request)
    {
        return await _dataService.UpdateAsync(request);
    }

    [HttpGet]
    [Route("api/data/tariffs/{id}")]
    public async Task<Tariff> Single(string id)
    {
        return await _dataService.SingleAsync<Tariff>(id);
    }
}