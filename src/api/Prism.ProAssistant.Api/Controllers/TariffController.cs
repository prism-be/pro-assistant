// -----------------------------------------------------------------------
//  <copyright file = "TariffController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Commands;
using Prism.ProAssistant.Business.Events;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Queries;
using IPublisher = Prism.ProAssistant.Business.Events.IPublisher;

namespace Prism.ProAssistant.Api.Controllers;

public class TariffController : Controller
{
    private readonly IMediator _mediator;
    private readonly IPublisher _publisher;

    public TariffController(IMediator mediator, IPublisher publisher)
    {
        _mediator = mediator;
        _publisher = publisher;
    }

    [Route("api/tariffs")]
    [HttpGet]
    public async Task<ActionResult<List<Tariff>>> FindMany()
    {
        var result = await _mediator.Send(new FindMany<Tariff>());
        return result
            .OrderBy(x => x.Name)
            .ToList()
            .ToActionResult();
    }

    [Route("api/tariff/{tariffId}")]
    [HttpGet]
    public async Task<ActionResult<Tariff>> FindOne(string tariffId)
    {
        var result = await _mediator.Send(new FindOne<Tariff>(tariffId));
        return result.ToActionResult();
    }

    [Route("api/tariff/{tariffId}")]
    [HttpDelete]
    public async Task RemoveOne(string tariffId)
    {
        await _mediator.Send(new RemoveOne<Tariff>(tariffId));
    }

    [Route("api/tariff")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Tariff tariff)
    {
        var result = await _mediator.Send(new UpsertOne<Tariff>(tariff));
        return result.ToActionResult();
    }
}