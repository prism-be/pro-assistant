// -----------------------------------------------------------------------
//  <copyright file = "SettingController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;

namespace Prism.ProAssistant.Api.Controllers;

public class SettingController : Controller
{
    private readonly IMediator _mediator;

    public SettingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("api/settings")]
    [HttpGet]
    public async Task<ActionResult<List<Setting>>> FindMany()
    {
        var result = await _mediator.Send(new FindMany<Setting>());
        return result.ToActionResult();
    }

    [Route("api/settings")]
    [HttpPost]
    public async Task SaveSettings([FromBody] List<Setting> settings)
    {
        await _mediator.Send(new SaveSettings(settings));
    }
}