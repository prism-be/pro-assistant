namespace Prism.ProAssistant.Api.Controllers.Data.Accounting;

using Core;
using Domain;
using Domain.Accounting.Document;
using Domain.Accounting.Document.Events;
using Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage;
using Storage.Events;

[Authorize]
public class DocumentController : Controller
{
    private readonly IEventStore _eventStore;
    private readonly IQueryService _queryService;

    public DocumentController(IEventStore eventStore, IQueryService queryService)
    {
        _eventStore = eventStore;
        _queryService = queryService;
    }

    [HttpPost]
    [Route("api/data/accounting/documents/delete")]
    public async Task<UpsertResult> Delete([FromBody] AccountingDocument request)
    {
        return await _eventStore.RaiseAndPersist<AccountingDocument>(new AccountingDocumentDeleted
        {
            StreamId = request.Id
        });
    }

    [HttpPost]
    [Route("api/data/accounting/documents/insert")]
    public async Task<UpsertResult> Insert([FromBody] AccountingDocument request)
    {
        return await _eventStore.RaiseAndPersist<AccountingDocument>(new AccountingDocumentCreated
        {
            StreamId = Identifier.GenerateString(),
            Amount = request.Amount,
            Date = request.Date,
            Title = request.Title
        });
    }

    [HttpGet]
    [Route("api/data/accounting/documents")]
    public async Task<IEnumerable<AccountingDocument>> List()
    {
        return await _queryService.ListAsync<AccountingDocument>();
    }

    [HttpGet]
    [Route("api/data/accounting/documents/{year}")]
    public async Task<IEnumerable<AccountingDocument>> List(int year)
    {
        var start = new DateTime(year, 1, 1);
        var end = start.AddYears(1);

        var filters = new List<Filter>
        {
            new("Date", start, FilterOperator.GreaterThanOrEqual),
            new("Date", end, FilterOperator.LessThan)
        };

        return await _queryService.SearchAsync<AccountingDocument>(filters.ToArray());
    }

    [HttpPost]
    [Route("api/data/accounting/documents/update")]
    public async Task<UpsertResult> Update([FromBody] AccountingDocument request)
    {
        return await _eventStore.RaiseAndPersist<AccountingDocument>(new AccountingDocumentUpdated
        {
            StreamId = request.Id,
            Amount = request.Amount,
            Date = request.Date,
            Title = request.Title
        });
    }
}