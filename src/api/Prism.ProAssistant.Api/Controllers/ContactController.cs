// -----------------------------------------------------------------------
//  <copyright file = "ContactController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Api.Controllers;

[Authorize]
public class ContactController : Controller
{
    private readonly ICrudService _crudService;
    private readonly ISearchContactsService _searchContactsService;

    public ContactController(ICrudService crudService, ISearchContactsService searchContactsService)
    {
        _crudService = crudService;
        _searchContactsService = searchContactsService;
    }

    [Route("api/contacts")]
    [HttpGet]
    public async Task<ActionResult<List<Contact>>> FindMany()
    {
        var result = await _crudService.FindMany<Contact>();
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
        var result = await _crudService.FindOne<Contact>(contactId);
        return result.ToActionResult();
    }

    [Route("api/contacts")]
    [HttpPost]
    public async Task<ActionResult<List<Contact>>> Search([FromBody] SearchContacts search)
    {
        var result = await _searchContactsService.Search(search.LastName, search.FirstName, search.PhoneNumber, search.BirthDate);
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
        var result = await _crudService.UpsertOne(contact);
        return result.ToActionResult();
    }
}