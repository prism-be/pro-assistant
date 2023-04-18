using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class ContactController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IStateProvider _stateProvider;

    public ContactController(IEventStore eventStore, IStateProvider stateProvider)
    {
        _eventStore = eventStore;
        _stateProvider = stateProvider;
    }

    [HttpPost]
    [Route("api/data/contacts/insert")]
    public async Task<UpsertResult> Insert([FromBody] Contact request)
    {
        request.Id = Identifier.GenerateString();

        return await _eventStore.RaiseAndPersist<ContactAggregator, Contact>(new ContactCreated
        {
            Contact = request
        });
    }

    [HttpGet]
    [Route("api/data/contacts")]
    public async Task<IEnumerable<Contact>> List()
    {
        var container = await _stateProvider.GetContainerAsync<Contact>();
        return await container.ListAsync();
    }

    [HttpPost]
    [Route("api/data/contacts/search")]
    public async Task<IEnumerable<Contact>> Search([FromBody] List<Filter> request)
    {
        var container = await _stateProvider.GetContainerAsync<Contact>();
        return await container.SearchAsync(request);
    }

    [HttpGet]
    [Route("api/data/contacts/{id}")]
    public async Task<Contact?> Single(string id)
    {
        var container = await _stateProvider.GetContainerAsync<Contact>();
        return await container.ReadAsync(id);
    }

    [HttpPost]
    [Route("api/data/contacts/update")]
    public async Task<UpsertResult> Update([FromBody] Contact request)
    {
        return await _eventStore.RaiseAndPersist<ContactAggregator, Contact>(new ContactUpdated
        {
            Contact = request
        });

        // TODO
        /*
        var result = await _eventService.ReplaceAsync(request);

        var filter = new FieldValue(nameof(Appointment.ContactId), request.Id);

        await _eventService.UpdateManyAsync<Appointment>(filter,
            new FieldValue(nameof(Appointment.FirstName), request.FirstName),
            new FieldValue(nameof(Appointment.LastName), request.LastName),
            new FieldValue(nameof(Appointment.BirthDate), request.BirthDate),
            new FieldValue(nameof(Appointment.PhoneNumber), request.PhoneNumber),
            new FieldValue(nameof(Appointment.Title), $"{request.LastName} {request.FirstName}")
        );

        return result;*/
    }
}