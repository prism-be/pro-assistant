// -----------------------------------------------------------------------
//  <copyright file = "AppointmentController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class AppointmentController : Controller
{
    private readonly IMediator _mediator;

    public AppointmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("api/appointments")]
    [HttpPost]
    public async Task<ActionResult<List<Appointment>>> Search([FromBody]SearchAppointments search)
    {
        var result = await _mediator.Send(search);
        return result
            .Where(x => x.State != (int)AppointmentState.Canceled)
            .ToList()
            .ToActionResult();
    }

    [Route("api/appointment/{appointmentId}")]
    [HttpGet]
    public async Task<ActionResult<Appointment>> FindOne(string appointmentId)
    {
        var result = await _mediator.Send(new FindOne<Appointment>(appointmentId));
        return result.ToActionResult();
    }
    
    [Route("api/appointment")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Appointment appointment)
    {
        if (string.IsNullOrWhiteSpace(appointment.PatientId))
        {
            var patient = new Patient
            {
                LastName = appointment.LastName,
                FirstName = appointment.FirstName
            };

            var patientId = await _mediator.Send(new UpsertOne<Patient>(patient));

            appointment.PatientId = patientId.Id;
        }

        var result = await _mediator.Send(new UpsertOne<Appointment>(appointment));
        return result.ToActionResult();
    }
}