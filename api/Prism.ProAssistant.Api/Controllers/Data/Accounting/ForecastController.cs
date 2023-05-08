﻿namespace Prism.ProAssistant.Api.Controllers.Data.Accounting;

using Core;
using Domain;
using Domain.Accounting.Forecast;
using Domain.Accounting.Forecast.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Storage.Events;

[Authorize]
public class ForecastController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public ForecastController(IEventStore eventStore, IQueryService queryService)
    {
        _eventStore = eventStore;
        _queryService = queryService;
    }

    [HttpPost]
    [Route("api/data/accounting/forecast/delete")]
    public async Task<UpsertResult> Delete([FromBody] ForecastInformation request)
    {
        return await _eventStore.RaiseAndPersist<Forecast>(new ForecastDeleted
        {
            StreamId = request.Id
        });
    }

    [HttpPost]
    [Route("api/data/accounting/forecast/insert")]
    public async Task<UpsertResult> Insert([FromBody] ForecastInformation request)
    {
        return await _eventStore.RaiseAndPersist<Forecast>(new ForecastCreated
        {
            StreamId = Identifier.GenerateString(),
            Title = request.Title
        });
    }

    [HttpGet]
    [Route("api/data/accounting/forecast")]
    public async Task<IEnumerable<Forecast>> List()
    {
        return await _queryService.ListAsync<Forecast>();
    }

    [HttpPost]
    [Route("api/data/accounting/forecast/update")]
    public async Task<UpsertResult> Update([FromBody] ForecastInformation request)
    {
        return await _eventStore.RaiseAndPersist<Forecast>(new ForecastUpdated
        {
            StreamId = request.Id,
            Title = request.Title
        });
    }

    public record ForecastInformation(string Id, string Title);
}