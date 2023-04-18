﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;
using Prism.ProAssistant.Storage;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class DocumentConfigurationController : Controller, IDataController<DocumentConfiguration>
{
    private readonly IQueryService _queryService;
    private readonly IEventService _eventService;

    public DocumentConfigurationController(IQueryService queryService, IEventService eventService)
    {
        _queryService = queryService;
        _eventService = eventService;
    }

    [HttpDelete]
    [Route("api/data/document-configurations/{id}")]
    public async Task<OperationResult?> Delete(string id)
    {
        return OperationResult.From(await _eventService.DeleteAsync<DocumentConfiguration>(id));
    }

    [HttpPost]
    [Route("api/data/document-configurations/insert")]
    public async Task<UpsertResult> Insert([FromBody] DocumentConfiguration request)
    {
        return await _eventService.CreateAsync(request);
    }

    [HttpGet]
    [Route("api/data/document-configurations")]
    public async Task<List<DocumentConfiguration>> List()
    {
        return await _queryService.ListAsync<DocumentConfiguration>();
    }

    [HttpPost]
    [Route("api/data/document-configurations/search")]
    public async Task<List<DocumentConfiguration>> Search([FromBody] List<SearchFilter> request)
    {
        return await _queryService.SearchAsync<DocumentConfiguration>(request);
    }

    [HttpGet]
    [Route("api/data/document-configurations/{id}")]
    public async Task<DocumentConfiguration?> Single(string id)
    {
        return await _queryService.SingleOrDefaultAsync<DocumentConfiguration>(id);
    }

    [HttpPost]
    [Route("api/data/document-configurations/update")]
    public async Task<UpsertResult> Update([FromBody] DocumentConfiguration request)
    {
        return await _eventService.ReplaceAsync(request);
    }
}