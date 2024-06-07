namespace Prism.ProAssistant.Api.Controllers.Data.Accounting;

using Core;
using Domain;
using Domain.Accounting.Document;
using Domain.Accounting.Document.Events;
using Helpers;
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
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _eventStore.RaiseAndPersist<AccountingDocument>(new AccountingDocumentDeleted
        {
            StreamId = request.Id,
            Date = request.Date
        });
    }

    [HttpGet]
    [Route("api/data/accounting/documents/next-number/{year:int}")]
    public async Task<NextNumber> GetNextNumber(int year)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddYears(1);

        var filters = new List<Filter>
        {
            new("Date", start, FilterOperator.GreaterThanOrEqual),
            new("Date", end, FilterOperator.LessThan)
        };

        return new NextNumber((await _queryService.MaxAsync<AccountingDocument, int?>(x => x.DocumentNumber, filters.ToArray()) ?? 0) + 1);
    }

    [HttpPost]
    [Route("api/data/accounting/documents/insert")]
    public async Task<UpsertResult> Insert([FromBody] AccountingDocument request)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _eventStore.RaiseAndPersist<AccountingDocument>(new AccountingDocumentCreated
        {
            StreamId = Identifier.GenerateString(),
            Document = request
        });
    }

    [HttpGet]
    [Route("api/data/accounting/documents")]
    public async Task<IEnumerable<AccountingDocument>> List()
    {
        return await _queryService.ListAsync<AccountingDocument>();
    }

    [HttpGet]
    [Route("api/data/accounting/documents/{year:int}")]
    public async Task<IEnumerable<AccountingDocument>> List(int year)
    {
        ModelStateHelper.Validate(ModelState.IsValid);
        
        if (year == 0)
        {
            return Array.Empty<AccountingDocument>();
        }

        var start = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
        ModelStateHelper.Validate(ModelState.IsValid);
        
        return await _eventStore.RaiseAndPersist<AccountingDocument>(new AccountingDocumentUpdated
        {
            StreamId = request.Id,
            Document = request
        });
    }

    public record NextNumber(int Number);
}