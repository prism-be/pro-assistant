// -----------------------------------------------------------------------
//  <copyright file = "DocumentsController.cs" company = "Prism">
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

public class DocumentsController : Controller
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("api/documents")]
    [HttpGet]
    public async Task<ActionResult<List<DocumentConfiguration>>> FindMany()
    {
        var result = await _mediator.Send(new FindMany<DocumentConfiguration>());
        return result
            .OrderBy(x => x.Name)
            .ToList()
            .ToActionResult();
    }

    [Route("api/documents/{documentId}")]
    [HttpGet]
    public async Task<ActionResult<DocumentConfiguration>> FindOne(string documentId)
    {
        var result = await _mediator.Send(new FindOne<DocumentConfiguration>(documentId));
        return result.ToActionResult();
    }

    [Route("api/documents/{documentId}")]
    [HttpDelete]
    public async Task RemoveOne(string documentId)
    {
        await _mediator.Send(new RemoveOne<DocumentConfiguration>(documentId));
    }

    [Route("api/documents")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] DocumentConfiguration documentConfiguration)
    {
        var result = await _mediator.Send(new UpsertOne<DocumentConfiguration>(documentConfiguration));
        return result.ToActionResult();
    }
}