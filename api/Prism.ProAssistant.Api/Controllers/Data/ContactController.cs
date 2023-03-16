using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class ContactController : Controller
{
    private readonly IDataService _dataService;

    public ContactController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpPost]
    [Route("api/data/contacts/insert")]
    public async Task<UpsertResult> Insert([FromBody] Contact request)
    {
        return await _dataService.InsertAsync(request);
    }

    [HttpGet]
    [Route("api/data/contacts")]
    public async Task<List<Contact>> List()
    {
        return await _dataService.ListAsync<Contact>();
    }

    [HttpPost]
    [Route("api/data/contacts/search")]
    public async Task<List<Contact>> Search([FromBody] List<SearchFilter> request)
    {
        return await _dataService.SearchAsync<Contact>(request);
    }

    [HttpGet]
    [Route("api/data/contacts/{id}")]
    public async Task<Contact?> Single(string id)
    {
        return await _dataService.SingleAsync<Contact>(id);
    }

    [HttpPost]
    [Route("api/data/contacts/update")]
    public async Task<UpsertResult> Update([FromBody] Contact request)
    {
        var result = await _dataService.UpdateAsync(request);

        var filter = Builders<Appointment>.Filter.Eq(x => x.Type, request.Id);
        var update = Builders<Appointment>.Update.Combine(
            Builders<Appointment>.Update.Set(x => x.FirstName, request.FirstName),
            Builders<Appointment>.Update.Set(x => x.LastName, request.LastName),
            Builders<Appointment>.Update.Set(x => x.BirthDate, request.BirthDate),
            Builders<Appointment>.Update.Set(x => x.PhoneNumber, request.PhoneNumber)
        );

        await _dataService.UpdateAsync(filter, update);

        return result;
    }
}