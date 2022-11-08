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

    [Route("api/setting/{settingId}")]
    [HttpGet]
    public async Task<ActionResult<Setting>> FindOne(string settingId)
    {
        var result = await _mediator.Send(new FindOne<Setting>(settingId));
        return result.ToActionResult();
    }

    [Route("api/setting")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Setting setting)
    {
        var result = await _mediator.Send(new UpsertOne<Setting>(setting));
        return result.ToActionResult();
    }
}