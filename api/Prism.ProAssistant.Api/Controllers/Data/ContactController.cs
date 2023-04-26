using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Core;
using Prism.Infrastructure.Providers;
using Prism.ProAssistant.Domain.DayToDay.Appointments;
using Prism.ProAssistant.Domain.DayToDay.Appointments.Events;
using Prism.ProAssistant.Domain.DayToDay.Contacts;
using Prism.ProAssistant.Domain.DayToDay.Contacts.Events;
using Prism.ProAssistant.Storage;
using Prism.ProAssistant.Storage.Events;

namespace Prism.ProAssistant.Api.Controllers.Data;

using Domain;

[Authorize]
public class ContactController : Controller
{
    private readonly IQueryService _queryService;
    private readonly IEventStore _eventStore;

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
        
        if (previous.FirstName != request.FirstName 
            || previous.LastName != request.LastName
            || previous.BirthDate != request.BirthDate
            || previous.PhoneNumber != request.PhoneNumber)
        {
            var appointmentContactUpdated = new AppointmentContactUpdated
            {
                StreamId = string.Empty,
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber,
                Title = $"{request.LastName} {request.FirstName}"
            };
            
            var filter = new Filter(nameof(Appointment.ContactId), request.Id);
            var appointments = await _queryService.SearchAsync<Appointment>(filter);
            
            foreach (var appointment in appointments)
            {
                appointmentContactUpdated.StreamId = appointment.Id;
                await _eventStore.RaiseAndPersist<Appointment>(appointmentContactUpdated);
            }
        }
        
        return result;

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