// -----------------------------------------------------------------------
//  <copyright file = "PatientController.cs" company = "Prism">
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
public class PatientController : Controller
{
    private readonly IMediator _mediator;

    public PatientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("api/patients")]
    [HttpGet]
    public async Task<ActionResult<List<Patient>>> FindMany()
    {
        var result = await _mediator.Send(new FindMany<Patient>());
        return result
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList()
            .ToActionResult();
    }

    [Route("api/patient/{patientId}")]
    [HttpGet]
    public async Task<ActionResult<Patient>> FindOne(string patientId)
    {
        var result = await _mediator.Send(new FindOne<Patient>(patientId));
        return result.ToActionResult();
    }

    [Route("api/patients")]
    [HttpPost]
    public async Task<ActionResult<List<Patient>>> SearchPatients([FromBody]SearchPatients search)
    {
        var result = await _mediator.Send(search);
        return result
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList()
            .ToActionResult();
    }

    [Route("api/patient")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Patient patient)
    {
        var result = await _mediator.Send(new UpsertOne<Patient>(patient));
        return result.ToActionResult();
    }
}