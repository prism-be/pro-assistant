﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.ProAssistant.Api.Models;
using Prism.ProAssistant.Api.Services;

namespace Prism.ProAssistant.Api.Controllers.Data;

[Authorize]
public class TariffController : Controller, IDataController<Tariff>
{
    private readonly IQueryService _queryService;
    private readonly IEventService _eventService;

    public TariffController(IQueryService queryService, IEventService eventService)
    {
        _queryService = queryService;
        _eventService = eventService;
    }

    [HttpPost]
    [Route("api/data/tariffs/insert")]
    public async Task<UpsertResult> Insert([FromBody] Tariff request)
    {
        return await _eventService.CreateAsync(request);
    }

    [HttpGet]
    [Route("api/data/tariffs")]
    public async Task<List<Tariff>> List()
    {
        return await _queryService.ListAsync<Tariff>();
    }

    [HttpPost]
    [Route("api/data/tariffs/search")]
    public async Task<List<Tariff>> Search([FromBody] List<SearchFilter> request)
    {
        return await _queryService.SearchAsync<Tariff>(request);
    }

    [HttpPost]
    [Route("api/data/tariffs/update")]
    public async Task<UpsertResult> Update([FromBody] Tariff request)
    {
        var updated = await _eventService.ReplaceAsync(request);

        var filter = new FieldValue(nameof(Appointment.TypeId), request.Id);

        await _eventService.UpdateManyAsync<Appointment>(filter,
            new FieldValue(nameof(Appointment.ForeColor), request.ForeColor),
            new FieldValue(nameof(Appointment.BackgroundColor), request.BackgroundColor)
        );

        return updated;
    }

    [HttpGet]
    [Route("api/data/tariffs/{id}")]
    public async Task<Tariff?> Single(string id)
    {
        return await _queryService.SingleOrDefaultAsync<Tariff>(id);
    }
}