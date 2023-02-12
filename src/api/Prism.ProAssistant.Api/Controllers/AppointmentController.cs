// -----------------------------------------------------------------------
//  <copyright file = "AppointmentController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using Prism.ProAssistant.Business.Security;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AppointmentController : Controller
{
    private readonly ICrudService _crudService;
    private readonly ISearchAppointmentsService _searchAppointmentsService;

    public AppointmentController(ICrudService crudService, ISearchAppointmentsService searchAppointmentsService)
    {
        _crudService = crudService;
        _searchAppointmentsService = searchAppointmentsService;
    }

    [Route("api/appointment/{appointmentId}")]
    [HttpGet]
    public async Task<ActionResult<Appointment>> FindOne(string appointmentId)
    {
        var result = await _crudService.FindOne<Appointment>(appointmentId);
        return result.ToActionResult();
    }

    [Route("api/appointments")]
    [HttpPost]
    public async Task<ActionResult<List<Appointment>>> Search([FromBody] SearchAppointments search)
    {
        var result = await _searchAppointmentsService.Search(search.StartDate, search.EndDate, search.ContactId);
        return result
            .Where(x => x.State != (int)AppointmentState.Canceled)
            .ToList()
            .ToActionResult();
    }

    [Route("api/appointment")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Appointment appointment)
    {
        if (string.IsNullOrWhiteSpace(appointment.ContactId))
        {
            var contact = new Contact
            {
                Id = Identifier.GenerateString(),
                LastName = appointment.LastName,
                FirstName = appointment.FirstName
            };

            await _crudService.UpsertOne(contact);

            appointment.ContactId = contact.Id;
        }

        var result = await _crudService.UpsertOne(appointment);

        await _crudService.UpdateProperty<Contact>(appointment.ContactId, nameof(Contact.BirthDate), appointment.BirthDate ?? string.Empty);
        await _crudService.UpdateProperty<Contact>(appointment.ContactId, nameof(Contact.PhoneNumber), appointment.PhoneNumber ?? string.Empty);

        return result.ToActionResult();
    }
}