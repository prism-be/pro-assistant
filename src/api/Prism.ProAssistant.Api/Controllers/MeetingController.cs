// -----------------------------------------------------------------------
//  <copyright file = "MeetingController.cs" company = "Prism">
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
public class MeetingController : Controller
{
    private readonly IMediator _mediator;

    public MeetingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("api/meetings")]
    [HttpGet]
    public async Task<ActionResult<List<Meeting>>> FindMany([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var result = await _mediator.Send(new SearchMeetings(startDate, endDate));
        return result.ToActionResult();
    }

    [Route("api/meeting/{meetingId}")]
    [HttpGet]
    public async Task<ActionResult<Meeting>> FindOne(string meetingId)
    {
        var result = await _mediator.Send(new FindOne<Meeting>(meetingId));
        return result.ToActionResult();
    }
    
    [Route("api/meeting")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Meeting meeting)
    {
        if (string.IsNullOrWhiteSpace(meeting.PatientId))
        {
            var patient = new Patient
            {
                LastName = meeting.LastName,
                FirstName = meeting.FirstName
            };

            var patientId = await _mediator.Send(new UpsertOne<Patient>(patient));

            meeting.PatientId = patientId.Id;
        }

        var result = await _mediator.Send(new UpsertOne<Meeting>(meeting));
        return result.ToActionResult();
    }
}