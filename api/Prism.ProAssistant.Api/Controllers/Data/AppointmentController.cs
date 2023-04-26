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
public class AppointmentController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public AppointmentController(IQueryService queryService, IEventStore eventStore)
    {
        _queryService = queryService;
        _eventStore = eventStore;
    }

    [HttpPost]
    [Route("api/data/appointments/insert")]
    public async Task<UpsertResult> Insert([FromBody] Appointment request)
    {
        await EnsureContact(request);

        return await _eventStore.RaiseAndPersist<Appointment>(new AppointmentCreated
        {
            Appointment = request
        });
    }

    [HttpGet]
    [Route("api/data/appointments")]
    public async Task<IEnumerable<Appointment>> List()
    {
        return await _queryService.ListAsync<Appointment>();
    }

    [HttpPost]
    [Route("api/data/appointments/search")]
    public async Task<IEnumerable<Appointment>> Search([FromBody] IEnumerable<Filter> request)
    {
        return await _queryService.SearchAsync<Appointment>(request.ToArray());
    }

    [HttpGet]
    [Route("api/data/appointments/{id}")]
    public async Task<Appointment?> Single(string id)
    {
        return await _queryService.SingleOrDefaultAsync<Appointment>(id);
    }

    [HttpPost]
    [Route("api/data/appointments/update")]
    public async Task<UpsertResult> Update([FromBody] Appointment request)
    {
        await EnsureContact(request);

        return await _eventStore.RaiseAndPersist<Appointment>(new AppointmentUpdated
        {
            Appointment = request
        });
    }

    private async Task EnsureContact(Appointment request)
    {
        if (request.ContactId == null)
        {
            var contact = new Contact
            {
                Id = Identifier.GenerateString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _eventStore.RaiseAndPersist<Contact>(new ContactCreated
            {
                Contact = contact
            });

            request.ContactId = result.Id;
        }
    }
}