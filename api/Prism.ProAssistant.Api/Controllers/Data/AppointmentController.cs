using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class AppointmentController : Controller, IDataController<Appointment>
{
    private readonly IQueryService _queryService;
    private readonly IEventService _eventService;

    public AppointmentController(IQueryService queryService, IEventService eventService)
    {
        _queryService = queryService;
        _eventService = eventService;
    }

    [HttpPost]
    [Route("api/data/appointments/insert")]
    public async Task<UpsertResult> Insert([FromBody] Appointment request)
    {
        await EnsureContact(request);

        return await _eventService.CreateAsync(request);
    }

    [HttpGet]
    [Route("api/data/appointments")]
    public async Task<List<Appointment>> List()
    {
        return await _queryService.ListAsync<Appointment>();
    }

    [HttpPost]
    [Route("api/data/appointments/search")]
    public async Task<List<Appointment>> Search([FromBody] List<SearchFilter> request)
    {
        return await _queryService.SearchAsync<Appointment>(request);
    }

    [HttpPost]
    [Route("api/data/appointments/update")]
    public async Task<UpsertResult> Update([FromBody] Appointment request)
    {
        await EnsureContact(request);

        return await _eventService.ReplaceAsync(request);
    }

    [HttpGet]
    [Route("api/data/appointments/{id}")]
    public async Task<Appointment?> Single(string id)
    {
        return await _queryService.SingleOrDefaultAsync<Appointment>(id);
    }

    private async Task EnsureContact(Appointment request)
    {
        if (request.ContactId == null)
        {
            var result = await _eventService.CreateAsync(new Contact
            {
                Id = Identifier.GenerateString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber
            });

            request.ContactId = result.Id;
        }
    }
}