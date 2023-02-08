// -----------------------------------------------------------------------
//  <copyright file = "DocumentsConfigurationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Extensions;
using Prism.ProAssistant.Business.Models;
using Prism.ProAssistant.Business.Services;

namespace Prism.ProAssistant.Api.Controllers;

public class DocumentsConfigurationController : Controller
{
    private readonly ICrudService _crudService;

    public DocumentsConfigurationController(ICrudService crudService)
    {
        _crudService = crudService;
    }

    [Route("api/documents-configuration")]
    [HttpGet]
    public async Task<ActionResult<List<DocumentConfiguration>>> FindMany()
    {
        var result = await _crudService.FindMany<DocumentConfiguration>();
        return result
            .OrderBy(x => x.Name)
            .ToList()
            .ToActionResult();
    }

    [Route("api/documents-configuration/{documentId}")]
    [HttpGet]
    public async Task<ActionResult<DocumentConfiguration>> FindOne(string documentId)
    {
        var result = await _crudService.FindOne<DocumentConfiguration>(documentId);
        return result.ToActionResult();
    }

    [Route("api/documents-configuration/{documentId}")]
    [HttpDelete]
    public async Task RemoveOne(string documentId)
    {
        await _crudService.RemoveOne<DocumentConfiguration>(documentId);
    }

    [Route("api/documents-configuration")]
    [HttpPost]
    public async Task<ActionResult<UpsertResult>> UpsertOne([FromBody] DocumentConfiguration documentConfiguration)
    {
        var result = await _crudService.UpsertOne(documentConfiguration);
        return result.ToActionResult();
    }
}