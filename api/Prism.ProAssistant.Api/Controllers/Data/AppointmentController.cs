namespace Prism.ProAssistant.Api.Controllers.Data;

using Core;
using Domain;
using Domain.DayToDay.Appointments;
using Domain.DayToDay.Appointments.Events;
using Domain.DayToDay.Contacts;
using Domain.DayToDay.Contacts.Events;
using Helpers;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Storage;
using Storage.Events;

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
    [Route("api/data/appointments/close")]
    public async Task<UpsertResult> Close([FromBody] AppointmentClosing request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        if (request.Payment != (int)PaymentTypes.Unpayed)
        {
            request.PaymentDate = DateTime.UtcNow;
        }

        return await _eventStore.RaiseAndPersist<Appointment>(new AppointmentClosed
        {
            Id = request.Id,
            Payment = request.Payment,
            PaymentDate = request.PaymentDate,
            State = request.State
        });
    }

    private async Task EnsureContact(AppointmentInformation request)
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

    [HttpPost]
    [Route("api/data/appointments/insert")]
    public async Task<UpsertResult> Insert([FromBody] AppointmentInformation request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        request.Id = Identifier.GenerateString();

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
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SearchAsync<Appointment>(request.ToArray());
    }

    [HttpGet]
    [Route("api/data/appointments/{id}")]
    public async Task<Appointment?> Single(string id)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _queryService.SingleOrDefaultAsync<Appointment>(id);
    }

    [HttpPost]
    [Route("api/data/appointments/update")]
    public async Task<UpsertResult> Update([FromBody] AppointmentInformation request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        await EnsureContact(request);

        return await _eventStore.RaiseAndPersist<Appointment>(new AppointmentUpdated
        {
            Appointment = request
        });
    }
}