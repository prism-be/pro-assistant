using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class TariffController : Controller, IDataController<Tariff>
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
    public async Task<UpsertResult> Update([FromBody] Tariff request)
    {
        var updated = await _dataService.ReplaceAsync(request);
        
        var filter = Builders<Appointment>.Filter.Eq(x => x.Type, request.Id);
        var update = Builders<Appointment>.Update.Combine(
            Builders<Appointment>.Update.Set(x => x.ForeColor, request.ForeColor),
            Builders<Appointment>.Update.Set(x => x.BackgroundColor, request.BackgroundColor)
        );

        await _dataService.UpdateManyAsync(filter, update);

        return updated;
    }

    [HttpGet]
    [Route("api/data/tariffs/{id}")]
    public async Task<Tariff?> Single(string id)
    {
        return await _dataService.SingleOrDefaultAsync<Tariff>(id);
    }
}