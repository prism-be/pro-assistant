using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class ContactController : Controller, IDataController<Contact>
{
    private readonly IDataService _dataService;
    private readonly IEventService _eventService;

    public ContactController(IDataService dataService, IEventService eventService)
    {
        _dataService = dataService;
        _eventService = eventService;
    }

    [HttpPost]
    [Route("api/data/contacts/insert")]
    public async Task<UpsertResult> Insert([FromBody] Contact request)
    {
        return await _eventService.CreateAsync(request);
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
        return await _dataService.SingleOrDefaultAsync<Contact>(id);
    }

    [HttpPost]
    [Route("api/data/contacts/update")]
    public async Task<UpsertResult> Update([FromBody] Contact request)
    {
        var result = await _eventService.ReplaceAsync(request);

        var filter = new FieldValue(nameof(Appointment.ContactId), request.Id);

        await _eventService.UpdateManyAsync<Appointment>(filter,
            new FieldValue(nameof(Appointment.FirstName), request.FirstName),
            new FieldValue(nameof(Appointment.LastName), request.LastName),
            new FieldValue(nameof(Appointment.BirthDate), request.BirthDate),
            new FieldValue(nameof(Appointment.PhoneNumber), request.PhoneNumber),
            new FieldValue(nameof(Appointment.Title), $"{request.LastName} {request.FirstName}")
        );

        return result;
    }
}