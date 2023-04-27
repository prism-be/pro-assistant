namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Storage.Events;

[Authorize]
public class ContactController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public ContactController(IQueryService queryService, IEventStore eventStore)
    {
        _eventStore = eventStore;
        _queryService = queryService;
    }

    [HttpPost]
    [Route("api/data/contacts/insert")]
    public async Task<UpsertResult> Insert([FromBody] Contact request)
    {
        request.Id = Identifier.GenerateString();

        return await _eventStore.RaiseAndPersist<Contact>(new ContactCreated
        {
            Contact = request
        });
    }

    [HttpGet]
    [Route("api/data/contacts")]
    public async Task<IEnumerable<Contact>> List()
    {
        return await _queryService.ListAsync<Contact>();
    }

    private async Task RebuildAppointments(string id)
    {
        var appointments = await _queryService.DistinctAsync<Appointment, string>(nameof(Appointment.Id), new Filter(nameof(Appointment.ContactId), id));

        foreach (var appointment in appointments)
        {
            await _eventStore.Persist<Appointment>(appointment);
        }
    }

    [HttpPost]
    [Route("api/data/contacts/search")]
    public async Task<IEnumerable<Contact>> Search([FromBody] Filter[] request)
    {
        return await _queryService.SearchAsync<Contact>(request);
    }

    [HttpGet]
    [Route("api/data/contacts/{id}")]
    public async Task<Contact?> Single(string id)
    {
        return await _queryService.SingleAsync<Contact>(id);
    }

    [HttpPost]
    [Route("api/data/contacts/update")]
    public async Task<UpsertResult> Update([FromBody] Contact request)
    {
        var previous = await _queryService.SingleAsync<Contact>(request.Id);

        var result = await _eventStore.RaiseAndPersist<Contact>(new ContactUpdated
        {
            Contact = request
        });

        if (previous.FirstName != request.FirstName || previous.LastName != request.LastName || previous.BirthDate != request.BirthDate || previous.PhoneNumber != request.PhoneNumber)
        {
            await RebuildAppointments(request.Id);
        }

        return result;
    }
}