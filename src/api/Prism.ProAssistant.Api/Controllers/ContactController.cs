// -----------------------------------------------------------------------
//  <copyright file = "ContactController.cs" company = "Prism">
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
public class ContactController : Controller
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Route("api/contacts")]
    [HttpGet]
    public async Task<ActionResult<List<Contact>>> FindMany()
    {
        var result = await _mediator.Send(new FindMany<Contact>());
        return result
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList()
            .ToActionResult();
    }

    [Route("api/contact/{contactId}")]
    [HttpGet]
    public async Task<ActionResult<Contact>> FindOne(string contactId)
    {
        var result = await _mediator.Send(new FindOne<Contact>(contactId));
        return result.ToActionResult();
    }

    [Route("api/contacts")]
    [HttpPost]
    public async Task<ActionResult<List<Contact>>> Search([FromBody]SearchContacts search)
    {
        var result = await _mediator.Send(search);
        return result
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToList()
            .ToActionResult();
    }

    [Route("api/contact")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] Contact contact)
    {
        var result = await _mediator.Send(new UpsertOne<Contact>(contact));
        return result.ToActionResult();
    }
}